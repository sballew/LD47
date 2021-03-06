﻿using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwinPixels.LD47
{
    public class BugSpawner : MonoBehaviour
    {
        public Bug bugPrefab;

        public BugType bugType = BugType.HealthAttacker;

        private float _lastSpawnTime = 0f;

        public float _spawnInterval = 2.2f;

        public int _bugsToSpawn = 2;

        public float _spawnDistance = 1.5f;

        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private int _health = 4;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            _lastSpawnTime = Time.time - 1.5f;
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
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
                
                
                // Spawn bug
                var bug = Instantiate(bugPrefab);

                // Position bug
                var spawnVector = Quaternion.Euler(0, 0, angle) * Vector2.up * _spawnDistance;
                var spawnPosition = transform.position + spawnVector;
                
                
                Debug.DrawLine(transform.position, spawnPosition, Color.green, 2f);

                bug.transform.position = spawnPosition;

            }
        }
    }
}