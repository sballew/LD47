using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class BugProjectile : MonoBehaviour
    {
        [SerializeField] private AudioClip _audioHitSound;
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            // GetComponent<AudioSource>().PlayOneShot(_audioHitSound, .1f);
            // AudioSource.PlayClipAtPoint(_audioHitSound, Camera.main.transform.position, .2f);
            GameManager.Instance.hudHitSoundSource.Play();
            Destroy(gameObject);

            if (other.gameObject.CompareTag("Health HUD"))
            {
                GameManager.Instance.ModifyHealth(-2);
            }
        }
    }
}