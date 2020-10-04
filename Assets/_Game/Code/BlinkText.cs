using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class BlinkText : MonoBehaviour
    {
        private TextMeshPro text;

        private void Awake()
        {
            text = GetComponent<TextMeshPro>();
        }

        private void OnEnable()
        {
            StartCoroutine("Blink");
        }

        private void OnDisable()
        {
            StopCoroutine("Blink");
        }

        private IEnumerator Blink()
        {
            yield return new WaitForSeconds(1f);
            text.enabled = false;
            yield return new WaitForSeconds(.75f);
            text.enabled = true;
            StartCoroutine("Blink");
        }
        
    }
}