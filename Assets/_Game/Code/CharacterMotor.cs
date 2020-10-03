using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TwinPixels.LD47
{
    public class CharacterMotor : MonoBehaviour
    {
        [Header("Collisions")]
        
        [SerializeField]
        private LayerMask wallLayer;

        [Header("Movement")]
        [SerializeField] private float speed = 1f;

        public PlayerInput CurrentInput;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;

        private bool _facingRight;
        private static readonly int AnimPropFacingLeft = Animator.StringToHash("FacingLeft");
        private static readonly int AnimPropFacingRight = Animator.StringToHash("FacingRight");
        private static readonly int AnimPropMoving = Animator.StringToHash("Moving");

        private void Start()
        {
            CurrentInput = new PlayerInput();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
            _facingRight = true;
        }

        private void Update()
        {
            ProcessMovement(this.CurrentInput);
            UpdateAnimParameters();
        }

        private void ProcessMovement(PlayerInput input)
        {
            Vector2 direction = Vector2.zero;
            if (input.Left)
                direction.x -= 1;
            if (input.Right)
                direction.x += 1;
            if (input.Up)
                direction.y += 1;
            if (input.Down)
                direction.y -= 1;
            
            
            Debug.Log("Direction: " + direction);
            
            _rigidbody2D.velocity = direction.normalized * speed;
            
             bool previousFacing = _facingRight;
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _facingRight = transform.position.x - mousePos.x < 0;
            

            if (_facingRight != previousFacing)
            {
                transform.localScale = new Vector3((_facingRight ? 1 : -1), transform.localScale.y, transform.localScale.z);
            }
        }

        private void UpdateAnimParameters()
        {
            var velocity = _rigidbody2D.velocity;
            _animator.SetBool(AnimPropMoving, velocity.magnitude > 0);
            _animator.SetBool(AnimPropFacingRight, velocity.x > 0);
            _animator.SetBool(AnimPropFacingLeft, velocity.x < 0);
        }
    }
}
