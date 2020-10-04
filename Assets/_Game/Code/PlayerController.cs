using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _attackInterval = .4f;
        private float _swordUpgradeIntervalMultiplier = 1.5f;

        [SerializeField] private SpriteRenderer _gemCarrySpriteRenderer;
        [SerializeField] private SpriteRenderer _keyCarrySpriteRenderer;
        
        private CharacterMotor _motor;

        private float _lastAttackTime = Mathf.NegativeInfinity;
        private float _lastBowAttackTime = Mathf.NegativeInfinity;

        private Animator _animator;

        [SerializeField]
        private GameObject _attackPrefab;
        
        
        [SerializeField]
        private float _bowAttackInterval = .8f;
        private float _bowUpgradeIntervalMultiplier = 1.5f;

        [SerializeField] private float _arrowVelocity = 3f;
        
        [SerializeField]
        private ArrowProjectile _bowProjectilePrefab;

        [SerializeField] private float _attackDistanceOffset = .5f;

        [SerializeField] private AudioClip _attackSound;

        private bool _canPickupGem;
        private SkillGem _gemToPickup;
        
        private bool _canPickupKey;
        private Key _keyToPickup;

        private bool _canPlaceGem;
        private SkillSlot _skillSlot;
        
        public bool ArrowUpgradeEnabled;
        public bool SwordUpgradeEnabled;
        
        public CharacterMotor Motor
        {
            get { return _motor; }
        }

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();
            _animator = GetComponentInChildren<Animator>();
        }

        public void Celebrate()
        {
            _animator.SetTrigger("Celebrate");
        }

        private void Update()
        {
            PlayerInput input = new PlayerInput()
            {
                Attack = Input.GetMouseButton(0),
                Attack2 = Input.GetMouseButtonDown(1),
                // Attack2 = false,
                Up = Input.GetKey(KeyCode.W),
                Down = Input.GetKey(KeyCode.S),
                Left = Input.GetKey(KeyCode.A),
                Right = Input.GetKey(KeyCode.D),
                Interact = Input.GetKeyDown(KeyCode.F)
            };

            _motor.CurrentInput = input;

            if (input.Interact && _canPickupGem && !GameManager.Instance.isPlayerCarryingGem && !GameManager.Instance.isPlayerCarryingKey)
            {
                PickupGem();
            }
            else if (input.Interact && _canPickupKey && !GameManager.Instance.isPlayerCarryingGem && !GameManager.Instance.isPlayerCarryingKey)
            {
                PickupKey();
            }
            else if (input.Interact && _canPlaceGem && GameManager.Instance.isPlayerCarryingGem)
            {
                PlaceGem();
            }
            if (input.Attack)
            {
                Attack();
            }
            if (input.Attack2)
            {
                AttackBow();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("SkillGem") && !GameManager.Instance.isPlayerCarryingGem)
            {
                _canPickupGem = true;
                _gemToPickup = other.GetComponent<SkillGem>();
            }
            
            else if (other.CompareTag("Key") && !GameManager.Instance.isPlayerCarryingGem)
            {
                _canPickupKey = true;
                _keyToPickup = other.GetComponent<Key>();
            }

            if (other.CompareTag("SkillSlot"))
            {
                _canPlaceGem = true;
                _skillSlot = other.GetComponent<SkillSlot>();
            }
            
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("SkillGem"))
            {
                _canPickupGem = false;
            }
            if (other.CompareTag("Key"))
            {
                _canPickupKey = false;
            }
        }

        private void PlaceGem()
        {
            DropGem();
            _skillSlot.SetSlotFill(true);
            GameManager.Instance.OnGemPlaced();
        }

        private void PickupGem()
        {
            _canPickupGem = false;
            GameManager.Instance.isPlayerCarryingGem = true;
            _animator.SetBool("Carrying", true);
            _gemCarrySpriteRenderer.enabled = true;
            Destroy(_gemToPickup.gameObject);
        }

        private void PickupKey()
        {
            _canPickupKey = false;
            GameManager.Instance.isPlayerCarryingKey = true;
            
            _animator.SetBool("Carrying", true);
            _keyCarrySpriteRenderer.enabled = true;
            Destroy(_keyToPickup.gameObject);
        }

        public void DropGem()
        {
            GameManager.Instance.isPlayerCarryingGem = false;
            _gemCarrySpriteRenderer.enabled = false;
            _animator.SetBool("Carrying", false);
        }
        
        public void DropKey()
        {
            GameManager.Instance.isPlayerCarryingKey = false;
            _keyCarrySpriteRenderer.enabled = false;
            _animator.SetBool("Carrying", false);
        }

        private void Attack()
        {
            if (Time.time - _lastAttackTime < (_attackInterval / (SwordUpgradeEnabled ? _swordUpgradeIntervalMultiplier : 1)))
            {
                return;
            }
            

            _lastAttackTime = Time.time;
            GameObject attackObj = Instantiate(_attackPrefab, transform);

            attackObj.transform.position = transform.position;

            if (SwordUpgradeEnabled)
            {
                attackObj.transform.localScale *= 2.5f;
            }
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float angleRads = Mathf.Atan2(mousePos.y - attackObj.transform.position.y,
                mousePos.x - attackObj.transform.position.x);
            float angleDeg = (180 / Mathf.PI) * angleRads;
            

            attackObj.transform.Translate(Vector3.right * _attackDistanceOffset, Space.Self);
            attackObj.transform.RotateAround(transform.position, Vector3.forward, angleDeg);
            
            AudioSource.PlayClipAtPoint(_attackSound, Camera.main.transform.position, .6f);
            
            Destroy(attackObj, .2f);
        }

        private void AttackBow()
        {
            
            if (Time.time - _lastBowAttackTime < (_bowAttackInterval / (ArrowUpgradeEnabled ? _bowUpgradeIntervalMultiplier : 1)))
            {
                return;
            }
            _lastBowAttackTime = Time.time;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float angleRads = Mathf.Atan2(mousePos.y - transform.position.y,
                mousePos.x - transform.position.x);
            
            float angleDeg = (180 / Mathf.PI) * angleRads;
            
            Vector2 velocityDirection = Vector2.zero;
            
            if (ArrowUpgradeEnabled)
            {
                SpawnArrow(Vector2.up * _arrowVelocity, 90, false);
                SpawnArrow(Vector2.down * _arrowVelocity, -90, false);
                SpawnArrow(Vector2.left * _arrowVelocity, 180, false);
                SpawnArrow(Vector2.right * _arrowVelocity, 0);
            }
            else
            {
                // Check if we're shooting up/left/down/right
                if (angleDeg >= -45 && angleDeg <= 45)
                {
                    // Right
                    SpawnArrow(Vector2.right * _arrowVelocity, 0);
                }
                else if (angleDeg >= 45 && angleDeg <= 135)
                {
                    // Up
                    SpawnArrow(Vector2.up * _arrowVelocity, 90);
                }
                else if (angleDeg >= -135 && angleDeg <= -45)
                {
                    // Down
                    SpawnArrow(Vector2.down * _arrowVelocity, -90);
                }
                else
                {
                    // Left
                    SpawnArrow(Vector2.left * _arrowVelocity, 180);
                }
            }
        }

        private void SpawnArrow(Vector2 velocity, float rotation, bool playSound = true)
        {
            ArrowProjectile arrowObj = Instantiate(_bowProjectilePrefab);
            Rigidbody2D arrowBody = arrowObj.GetComponent<Rigidbody2D>();
            
            arrowObj.transform.Rotate(Vector3.forward, rotation);
            
            Vector2 spawnOffset = _attackDistanceOffset * velocity.normalized;
            arrowObj.transform.position = new Vector2(transform.position.x + spawnOffset.x, transform.position.y + spawnOffset.y);
            
            arrowBody.velocity = velocity;

            Debug.DrawLine(arrowObj.transform.position, (Vector2)arrowObj.transform.position + (arrowBody.velocity.normalized * 3f), Color.green, 5f);

            if (playSound)
            {
                AudioSource.PlayClipAtPoint(_attackSound, Camera.main.transform.position, .5f);
            }

            Destroy(arrowObj.gameObject, 1f);
        }
    }
}