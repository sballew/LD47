using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwinPixels.LD47
{
    public class Bug : MonoBehaviour
    {
        /*
         * Assign a bug's type or role.
         * The bug then implements basic path finding to get to their objective.
         */

        [SerializeField] private BugType _bugType;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _attackInterval = 0.5f;
        [SerializeField] private Rigidbody2D _attackProjectilePrefab;
        [SerializeField] private float _projectileSpeed = 1.5f;

        [SerializeField] private AudioClip _projectileShootSound;

        private Vector2 _moveToPosition;
        private Vector2 _attackTargetPosition;

        private bool isAtPosition = false;

        private float _lastAttackTime = Mathf.NegativeInfinity;
        
        

        private void Start()
        {
            // Pick where to navigate to.
            if (_bugType == BugType.HealthAttacker)
            {
                _moveToPosition = GetRandomPosition(GameManager.Instance.healthAttackFromZone);
                _attackTargetPosition = GetRandomPosition(GameManager.Instance.healthAttackTargetZone);
            }
        }

        private void FixedUpdate()
        {
            if (!isAtPosition)
            {
                // Continue moving to position
                transform.position = Vector2.MoveTowards(transform.position, _moveToPosition, _speed * Time.deltaTime);

                isAtPosition = Mathf.Approximately(Mathf.Abs(transform.position.x - _moveToPosition.x), 0f) &&
                               Mathf.Approximately(Mathf.Abs(transform.position.y - _moveToPosition.y), 0f);

                if (isAtPosition)
                {
                    Debug.Log("Arrived at target position");
                }
            }

            if (isAtPosition)
            {
                switch (_bugType)
                {
                    case BugType.HealthAttacker:
                        UpdateHealthAttacker();
                        break;
                    default:
                        Debug.Log("Bug type not implemented: " + _bugType);
                        break;
                }
            }
        }

        private void UpdateHealthAttacker()
        {
            // Attack target
            if (Time.time - _lastAttackTime < _attackInterval)
            {
                return;
            }

            _lastAttackTime = Time.time;

            Rigidbody2D projectile = Instantiate(_attackProjectilePrefab);
            projectile.transform.position = transform.position;

            projectile.velocity = (_attackTargetPosition - (Vector2)transform.position).normalized * _projectileSpeed;
            
            Destroy(projectile, 5f);
            
            AudioSource.PlayClipAtPoint(_projectileShootSound, Camera.main.transform.position, .1f);

        }

        private Vector2 GetRandomPosition(BoxCollider2D zone)
        {

            var bounds = zone.bounds;
            float minX = bounds.center.x - bounds.extents.x;
            float maxX = bounds.center.x + bounds.extents.x;
            float minY = bounds.center.y - bounds.extents.y;
            float maxY = bounds.center.y + bounds.extents.y;
            
            Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            
            Debug.DrawLine(transform.position, randomPosition, Color.red, 5f);

            return randomPosition;
        }
    }
}