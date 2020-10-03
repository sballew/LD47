using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TwinPixels.LD47
{
    public class GameManager : MonoBehaviour
    {
        public BoxCollider2D healthAttackFromZone;
        public BoxCollider2D healthAttackTargetZone;

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }
    }
}