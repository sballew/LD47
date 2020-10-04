using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwinPixels.LD47
{
    public class BugSpawner : MonoBehaviour
    {
        public Bug healthAttackerBugPrefab;
        public Bug skillStealerBugPrefab;

        public BugType bugType = BugType.HealthAttacker;

        private float _lastSpawnTime = 0f;

        private float _spawnInterval = 2.5f;

        private int _bugsToSpawn = 2;

        private float _spawnDistance = 1f;

        private SpriteRenderer _spriteRenderer;

        private int _health = 4;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (bugType == BugType.SkillStealer)
            {
                _spawnInterval = 10f;
            }
        }

        private void Start()
        {
            _lastSpawnTime = Time.time - (_spawnInterval / 2f);
        }

        public void TakeDamage()
        {
            _health--;
            if (_health <= 0)
            {
                GameManager.Instance.OnSpawnerKilled();
                Destroy(gameObject);
                return;
            }

            StartCoroutine(nameof(Flicker));
        }
        
        private IEnumerator Flicker()
        {
            _spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.color = Color.white;
        }

        private void Update()
        {
            if (Time.time - _lastSpawnTime < _spawnInterval)
            {
                return;
            }

            _lastSpawnTime = Time.time;

            int actualBugsToSpawn = bugType == BugType.HealthAttacker ? _bugsToSpawn : 1;

            for (var i = 0; i < actualBugsToSpawn; i++)
            {
                // Pick random angle
                float angle = Random.Range(0, 360);

                Bug bugPrefabToSpawn = null;
                if (bugType == BugType.HealthAttacker)
                {
                    bugPrefabToSpawn = healthAttackerBugPrefab;
                }
                else if (bugType == BugType.SkillStealer)
                {
                    bugPrefabToSpawn = skillStealerBugPrefab;
                }
                else
                {
                    Debug.LogError("Unknown bug type for spawner: " + bugType);
                }
                
                // Spawn bug
                var bug = Instantiate(bugPrefabToSpawn);

                // Position bug
                var spawnVector = Quaternion.Euler(0, 0, angle) * Vector2.up * _spawnDistance;
                var spawnPosition = transform.position + spawnVector;
                
                
                Debug.DrawLine(transform.position, spawnPosition, Color.green, 2f);

                bug.transform.position = spawnPosition;

            }
        }
    }
}