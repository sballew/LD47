using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class Door : MonoBehaviour
    {
        [SerializeField]
        private GameObject _closedDoor;
        
        [SerializeField]
        private GameObject _openedDoor;

        private bool _isLocked = true;
        
        public void OpenDoor()
        {
            _closedDoor.SetActive(false);
            _openedDoor.SetActive(true);
            _isLocked = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (_isLocked && GameManager.Instance.isPlayerCarryingKey)
            {
                OpenDoor();
                GameManager.Instance.WinTheGame();
            }
        }
    }
}