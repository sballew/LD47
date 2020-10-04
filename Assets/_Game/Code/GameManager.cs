using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwinPixels.LD47
{
    public class GameManager : MonoBehaviour
    {
        public BoxCollider2D healthAttackFromZone;
        public BoxCollider2D healthAttackTargetZone;

        [SerializeField] private SpriteMask healthBarMask;

        [SerializeField] private BugSpawner spawnerPrefab;
        [SerializeField] private BoxCollider2D spawnerZone;

        private bool _spawnersActive = false;
        
        private int _currentHealth = 100;

        private float _spawnerCurrentInterval = 5f;
        private float _lastSpawnerTime = Mathf.NegativeInfinity;

        public bool isPlayerCarryingGem = false;

        public PlayerController player;

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            
            Instance = this;
            player = FindObjectOfType<PlayerController>();
            StartSpawners();
        }

        private void Update()
        {
            if (_spawnersActive)
            {
                if (Time.time - _lastSpawnerTime >= _spawnerCurrentInterval)
                {
                    _lastSpawnerTime = Time.time;
                    CreateNewSpawner();
                }
            }
        }

        private void CreateNewSpawner()
        {
            // Pick location
            Vector2 spawnerLocation = GetRandomPosition(spawnerZone);
            
            // Create spawner
            var spawner = Instantiate(spawnerPrefab);

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

        public void ModifyHealth(int amount)
        {
            _currentHealth += amount;
            if (_currentHealth < 0)
            {
                // Debug.Log("Player is dead");
                _currentHealth = 0;
            }
            
            Transform maskScaler = healthBarMask.transform.parent;
            
            maskScaler.localScale = new Vector3(_currentHealth / 100f, 1,
                1);
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