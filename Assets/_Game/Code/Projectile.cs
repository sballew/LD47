using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private AudioClip _audioHitSound;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log("Projectile collided with " + other.gameObject.name);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Projectile triggered with " + other.gameObject.name);
            // GetComponent<AudioSource>().PlayOneShot(_audioHitSound, .1f);
            AudioSource.PlayClipAtPoint(_audioHitSound, Camera.main.transform.position, .1f);
            Destroy(gameObject);
        }
    }
}