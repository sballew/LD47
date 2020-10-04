using UnityEngine;

namespace TwinPixels.LD47
{
    public class Door : MonoBehaviour
    {
        private GameObject _closedDoor;
        private GameObject _openedDoor;
        
        public void OpenDoor()
        {
            _closedDoor.SetActive(false);
            _openedDoor.SetActive(true);
        }
    }
}