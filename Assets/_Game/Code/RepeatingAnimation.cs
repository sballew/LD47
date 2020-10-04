using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class RepeatingAnimation : MonoBehaviour
    {
        public string animationTriggerName;

        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            InvokeRepeating(nameof(PlayAnimation), 3f, 3f);
        }

        private void PlayAnimation()
        {
            _animator.SetTrigger(animationTriggerName);
        }
    }
}