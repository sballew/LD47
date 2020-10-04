using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _attackInterval = .2f;

        [SerializeField] private SpriteRenderer _gemCarrySpriteRenderer;
        
        private CharacterMotor _motor;

        private float _lastAttackTime = Mathf.NegativeInfinity;

        private Animator _animator;

        [SerializeField]
        private GameObject _attackPrefab;

        [SerializeField] private float _attackDistanceOffset = .5f;

        [SerializeField] private AudioClip _attackSound;

        private bool _canPickupGem;
        private SkillGem _gemToPickup;

        private bool _canPlaceGem;
        private SkillSlot _skillSlot;
        
        public CharacterMotor Motor
        {
            get { return _motor; }
        }

        private void Start()
        {
            _motor = GetComponent<CharacterMotor>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            PlayerInput input = new PlayerInput()
            {
                Attack = Input.GetMouseButtonDown(0),
                Up = Input.GetKey(KeyCode.W),
                Down = Input.GetKey(KeyCode.S),
                Left = Input.GetKey(KeyCode.A),
                Right = Input.GetKey(KeyCode.D),
                Interact = Input.GetKeyDown(KeyCode.F)
            };

            _motor.CurrentInput = input;

            if (input.Interact && _canPickupGem)
            {
                Debug.Log("Picking up gem");
                PickupGem();
            }
            else if (input.Interact && _canPlaceGem)
            {
                Debug.Log("Placing gem in slot " + _skillSlot.gameObject.name);
                PlaceGem();
            }
            else if (input.Attack)
            {
                Attack();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Player entered trigger: " + other.gameObject.name);
            if (other.CompareTag("SkillGem") && !GameManager.Instance.isPlayerCarryingGem)
            {
                _canPickupGem = true;
                _gemToPickup = other.GetComponent<SkillGem>();
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
        }

        private void PlaceGem()
        {
            DropGem();
            _skillSlot.SetSlotFill(true);
        }

        private void PickupGem()
        {
            Debug.Log("Picking up gem: " + _gemToPickup);
            _canPickupGem = false;
            GameManager.Instance.isPlayerCarryingGem = true;
            _animator.SetBool("Carrying", true);
            _gemCarrySpriteRenderer.enabled = true;
            Destroy(_gemToPickup.gameObject);
        }

        private void DropGem()
        {
            GameManager.Instance.isPlayerCarryingGem = false;
            _gemCarrySpriteRenderer.enabled = false;
            _animator.SetBool("Carrying", false);
        }

        private void Attack()
        {
            if (Time.time - _lastAttackTime < _attackInterval)
            {
                return;
            }
            

            _lastAttackTime = Time.time;
            GameObject attackObj = Instantiate(_attackPrefab, transform);

            attackObj.transform.position = transform.position;
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float angleRads = Mathf.Atan2(mousePos.y - attackObj.transform.position.y,
                mousePos.x - attackObj.transform.position.x);
            float angleDeg = (180 / Mathf.PI) * angleRads;
            

            attackObj.transform.Translate(Vector3.right * _attackDistanceOffset, Space.Self);
            attackObj.transform.RotateAround(transform.position, Vector3.forward, angleDeg);
            
            AudioSource.PlayClipAtPoint(_attackSound, Camera.main.transform.position, .5f);
            
            Destroy(attackObj, .2f);
        }
    }
}