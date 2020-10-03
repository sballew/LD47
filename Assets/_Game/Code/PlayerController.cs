using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterMotor _motor;
        
        private void Start()
        {
            _motor = GetComponent<CharacterMotor>();
        }

        private void Update()
        {
            PlayerInput input = new PlayerInput()
            {
                Attack = Input.GetMouseButton(0),
                Up = Input.GetKey(KeyCode.W),
                Down = Input.GetKey(KeyCode.S),
                Left = Input.GetKey(KeyCode.A),
                Right = Input.GetKey(KeyCode.D),
            };

            _motor.CurrentInput = input;
        }
    }
}