using UnityEngine;

namespace TwinPixels.LD47
{
    public class Key : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _pickupIndicator;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Key trigger: " + other.gameObject.name);
            if (other.CompareTag("Player"))
            {
                _pickupIndicator.enabled = true;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _pickupIndicator.enabled = false;
            }
        }
    }
}