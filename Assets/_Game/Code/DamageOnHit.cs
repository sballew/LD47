using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class DamageOnHit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("DamageOnHit checking trigger against " + other.gameObject.name);
            if (other.CompareTag("Bug"))
            {
                Bug bug = other.GetComponent<Bug>();
                if (bug != null)
                {
                    bug.TakeDamage();
                }
            }
            else if (other.CompareTag("Spawner"))
            {
                Debug.Log("Hit spawner");
                BugSpawner spawner = other.GetComponent<BugSpawner>();
                spawner.TakeDamage();
            }
        }
    }
}