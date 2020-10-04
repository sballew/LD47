using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class SkillGem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _pickupIndicator;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Skill gem detected trigger enter: " + other.gameObject.name);
            if (other.CompareTag("Player"))
            {
                Debug.Log("PLAYER DETECTED");
                _pickupIndicator.enabled = true;
            }
            else
            {
                Debug.Log("NOT PLAYER");
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log("Skill gem detected trigger exit: " + other.gameObject.name);
            if (other.CompareTag("Player"))
            {
                _pickupIndicator.enabled = false;
            }
        }
    }
}