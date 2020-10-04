using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwinPixels.LD47
{
    public class GameManager : MonoBehaviour
    {
        public bool SpawnOnStart = true;

        public AudioClip MusicGameplay;
        public AudioClip MusicWin;
        public AudioClip MusicLose;

        [SerializeField]
        private GameObject _keySpawnPoint;

        [SerializeField] private Key _keyPrefab;

        [SerializeField]
        private AudioSource _musicSource;

        public AudioSource hudHitSoundSource;
        
        public BoxCollider2D healthAttackFromZone;
        public BoxCollider2D healthAttackTargetZone;

        public SkillSlot[] UpgradeSlots;

        [SerializeField] private SpriteMask healthBarMask;

        [SerializeField] private BugSpawner healthAttackerSpawnerPrefab;
        [SerializeField] private BugSpawner skillThiefSpawnerPrefab;
        [SerializeField] private BoxCollider2D spawnerZone;

        [SerializeField] private GameObject gameOverVisual;
        [SerializeField] private GameObject gameWinVisual;

        private bool _spawnersActive = false;
        
        private int _currentHealth = 100;

        private float _skillThiefSpawnerChance = .3f;

        private int _spawnersKilled = 0;
        private int _bugsKilled = 0;

        private int _playerLivesLeft = 3;

        [SerializeField]
        private int _maxSpawners = 6;
        
        private int _spawnersCreatedSoFar = 0;
        private float _spawnerCurrentInterval = 5f;
        private float _lastSpawnerTime = -2f;
        private float _doubleSpawnerChance = .15f;

        private float skillGemSpawnInterval = 23f;
        private float lastSkillGemSpawn = -13f;
        public SkillGem skillGemPrefab;

        public bool isPlayerCarryingGem = false;
        public bool isPlayerCarryingKey = false;

        public PlayerController player;

        public TextMeshPro scoreText;

        public SkillSlot[] LifeSlotObjects;

        public static GameManager Instance { get; private set; }
        
        public SkillSlot GetRandomFilledSkill()
        {
            List<SkillSlot> filledSlots = new List<SkillSlot>();
            for (int i = 0; i < UpgradeSlots.Length; i++)
            {
                if (UpgradeSlots[i].IsFilled)
                {
                    filledSlots.Add(UpgradeSlots[i]);
                }
            }

            if (filledSlots.Count <= 0)
            {
                return null;
            }

            return filledSlots[Random.Range(0, filledSlots.Count)];
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            if (LifeSlotObjects == null || LifeSlotObjects.Length != 3)
            {
                throw new Exception("Life slot array must have 3 elements.");
            }
            
            Instance = this;
            player = FindObjectOfType<PlayerController>();
            if (SpawnOnStart)
            {
                StartSpawners();
            }
            
            gameOverVisual.SetActive(false);

            _musicSource.loop = true;
            _musicSource.clip = MusicGameplay;
            _musicSource.Play();

        }

        private void Update()
        {
            if (_spawnersActive)
            {
                if (Time.time - _lastSpawnerTime >= _spawnerCurrentInterval)
                {
                    _lastSpawnerTime = Time.time;
                    
                    
                    int activeSpawners = FindObjectsOfType<BugSpawner>().Length;


                    if (activeSpawners < _maxSpawners)
                    {
                                            
                        CreateNewSpawner();
                        _spawnersCreatedSoFar++;
                        
                        if (_spawnersCreatedSoFar == 3 || (_spawnersCreatedSoFar > 3 && Random.Range(0f, 1f) <= _doubleSpawnerChance))
                        {
                            CreateNewSpawner();
                        }

                        // Every 6 spawners, increase the rate of new spawners.
                        if (_spawnersCreatedSoFar % 5 == 0)
                        {
                            _spawnerCurrentInterval = Mathf.Max(_spawnerCurrentInterval - .25f, 2.5f);
                        }
                    
                        // See if we want to spawn a skill thief spawner as well
                        if (Random.Range(0f, 1f) <= _skillThiefSpawnerChance)
                        {
                            if (GetRandomFilledSkill() != null)
                            {
                                CreateNewSpawner(BugType.SkillStealer);
                            }
                        }
                    }
                }

                if (Time.time - lastSkillGemSpawn >= skillGemSpawnInterval)
                {
                    lastSkillGemSpawn = Time.time;

                    int activeGems = FindObjectsOfType<SkillGem>().Length;
                    if (activeGems < 2)
                    {
                        SpawnSkillGem();    
                    }
                }
            }
        }

        public void OnGemPlaced()
        {
            // See if all slots are filled
            foreach (var upgradeSlot in UpgradeSlots)
            {
                if (!upgradeSlot.IsFilled)
                {
                    return;
                }
            }
            
            // All slots filled.
            StartCoroutine(nameof(SpawnKey));
        }

        private IEnumerator SpawnKey()
        {
            if (isPlayerCarryingKey || FindObjectOfType<Key>() != null)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(2f);
            var key = Instantiate(_keyPrefab);
            key.transform.position = _keySpawnPoint.transform.position;
        }

        public void OnBugKilled()
        {
            _bugsKilled++;
            UpdateScore();
        }

        public void OnSpawnerKilled()
        {
            _spawnersKilled++;
            UpdateScore();
        }

        private void SpawnSkillGem()
        {
            Vector2 spawnPoint = GetRandomPosition(spawnerZone);
            var gem = Instantiate(skillGemPrefab);
            gem.transform.position = spawnPoint;
        }

        private void UpdateScore()
        {
            int score = _bugsKilled + (_spawnersKilled * 10);
            scoreText.text = $"{score:n0}";
        }

        private void CreateNewSpawner(BugType bugType = BugType.HealthAttacker)
        {
            // Pick location
            Vector2 spawnerLocation = GetRandomPosition(spawnerZone);
            
            // Create spawner
            BugSpawner prefabToSpawn;
            if (bugType == BugType.HealthAttacker)
            {
                prefabToSpawn = healthAttackerSpawnerPrefab;
            }
            else if (bugType == BugType.SkillStealer)
            {
                prefabToSpawn = skillThiefSpawnerPrefab;
            }
            else
            {
                throw new Exception("Unknown spawner type: " + bugType);
            }
            
            var spawner = Instantiate(prefabToSpawn);

            spawner.bugType = bugType;
            spawner.transform.position = spawnerLocation;

            float randomRotation = Random.Range(0f, 1f);
            if (randomRotation < .25f)
            {
                spawner.transform.Rotate(Vector3.forward, 90);
            }
            else if (randomRotation < .5f)
            {
                spawner.transform.Rotate(Vector3.forward, 180);
            }
            else if (randomRotation < .75f)
            {
                spawner.transform.Rotate(Vector3.forward, 270);
            }
        }

        private void StartSpawners()
        {
            _spawnersActive = true;
        }
        
        private void StopSpawners()
        {
            _spawnersActive = false;
        }


        public void ModifyHealth(int amount)
        {
            _currentHealth += amount;
            if (_currentHealth < 0)
            {
                _currentHealth = 0;
                if (_playerLivesLeft <= 1)
                {
                    _playerLivesLeft--;
                    UpdateLivesRemainingSlots();
                    GameOver();
                }
                else
                {
                    _playerLivesLeft--;
                    UpdateLivesRemainingSlots();
                    _currentHealth = 100;
                }
            }
            
            Transform maskScaler = healthBarMask.transform.parent;
            
            maskScaler.localScale = new Vector3(_currentHealth / 100f, 1,
                1);
        }

        public void WinTheGame()
        {
            StopSpawners();
            
            player.DropGem();
            player.DropKey();
            
            
            var spawners = FindObjectsOfType<BugSpawner>();
            foreach (var spawner in spawners)
            {
                Destroy(spawner.gameObject);
            }

            var bugs = FindObjectsOfType<Bug>();
            foreach (var bug in bugs)
            {
                Destroy(bug.gameObject);
            }

            var projectiles = FindObjectsOfType<BugProjectile>();
            foreach (var projectile in projectiles)
            {
                Destroy(projectile.gameObject);
            }
            
            var gems = FindObjectsOfType<SkillGem>();
            foreach (var gem in gems)
            {
                Destroy(gem.gameObject);
            }
            
            gameWinVisual.SetActive(true);
            
            _musicSource.Stop();
            
            player.Celebrate();

            StartCoroutine(nameof(PlayVictory));
        }
        private IEnumerator PlayVictory()
        {
            yield return new WaitForSeconds(0.5f);
            
            _musicSource.loop = false;
            _musicSource.clip = MusicWin;
            _musicSource.Play();
        }
        

        private void GameOver()
        {
            StopSpawners();
            
            player.DropGem();
            player.DropKey();
            
            
            var spawners = FindObjectsOfType<BugSpawner>();
            foreach (var spawner in spawners)
            {
                Destroy(spawner.gameObject);
            }

            var bugs = FindObjectsOfType<Bug>();
            foreach (var bug in bugs)
            {
                Destroy(bug.gameObject);
            }

            var projectiles = FindObjectsOfType<BugProjectile>();
            foreach (var projectile in projectiles)
            {
                Destroy(projectile.gameObject);
            }
            
            var gems = FindObjectsOfType<SkillGem>();
            foreach (var gem in gems)
            {
                Destroy(gem.gameObject);
            }
            
            gameOverVisual.SetActive(true);
            
            _musicSource.Stop();

            StartCoroutine("RestartGame");
        }

        private IEnumerator RestartGame()
        {
            yield return new WaitForSeconds(0.5f);
            
            _musicSource.loop = false;
            _musicSource.clip = MusicLose;
            _musicSource.Play();
            
            yield return new WaitForSeconds(4.5f);
            gameOverVisual.SetActive(false);
            
            _musicSource.loop = true;
            _musicSource.clip = MusicGameplay;
            _musicSource.Play();

            _currentHealth = 100;
            _playerLivesLeft = 3;
            UpdateLivesRemainingSlots();
            
            Transform maskScaler = healthBarMask.transform.parent;
            maskScaler.localScale = new Vector3(_currentHealth / 100f, 1,
                1);

            foreach (var slot in UpgradeSlots)
            {
                slot.SetSlotFill(false);
            }

            _bugsKilled = 0;
            _spawnersKilled = 0;
            UpdateScore();

            lastSkillGemSpawn = Time.time - 13f;
            _lastSpawnerTime = Time.time - 2f;
            
            StartSpawners();
        }

        private void UpdateLivesRemainingSlots()
        {
            for (var i = 0; i < LifeSlotObjects.Length; i++)
            {
                // Fill slot if the player has this life available
                bool fillSlot = _playerLivesLeft >= (i + 1);
                LifeSlotObjects[i].SetSlotFill(fillSlot);
            }
        }
        
        public static Vector2 GetRandomPosition(BoxCollider2D zone)
        {
            var bounds = zone.bounds;
            float minX = bounds.min.x;
            float maxX = bounds.max.x;
            float minY = bounds.min.y;
            float maxY = bounds.max.y;

            Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            return randomPosition;
        }
    }
}