using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class BugProjectile : MonoBehaviour
    {
        [SerializeField] private AudioClip _audioHitSound;
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Projectile triggered with " + other.gameObject.name);
            // GetComponent<AudioSource>().PlayOneShot(_audioHitSound, .1f);
            AudioSource.PlayClipAtPoint(_audioHitSound, Camera.main.transform.position, .2f);
            Destroy(gameObject);

            if (other.gameObject.CompareTag("Health HUD"))
            {
                GameManager.Instance.ModifyHealth(-2);
            }
        }
    }
}