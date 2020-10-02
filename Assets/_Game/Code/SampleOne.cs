using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class SampleOne : MonoBehaviour
    {
        public float interval = 2f;
        
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine("DoRotate");
        }

        private IEnumerator DoRotate()
        {
            yield return new WaitForSeconds(interval);
            transform.Rotate(0, 0, 90);
            StartCoroutine("DoRotate");
        }
    }
}
