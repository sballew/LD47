using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class DamageOnHit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("DamageOnHit checking trigger against " + other.gameObject.name);
            Bug bug = other.GetComponent<Bug>();
            Debug.Log("Hit bug: " + bug);
            if (bug != null)
            {
                bug.TakeDamage();
            }
        }
    }
}