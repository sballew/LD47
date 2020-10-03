using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _attackInterval = .2f;
        
        private CharacterMotor _motor;

        private float _lastAttackTime = Mathf.NegativeInfinity;

        [SerializeField]
        private GameObject _attackPrefab;

        [SerializeField] private float _attackDistanceOffset = .5f;

        [SerializeField] private AudioClip _attackSound;
        
        private void Start()
        {
            _motor = GetComponent<CharacterMotor>();
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
            };

            _motor.CurrentInput = input;

            if (input.Attack)
            {
                Attack();
            }
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
            
            AudioSource.PlayClipAtPoint(_attackSound, Camera.main.transform.position, .3f);
            
            Destroy(attackObj, 1f);

        }
    }
}