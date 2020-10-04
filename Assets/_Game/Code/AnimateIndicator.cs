using System;
using System.Collections;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class AnimateIndicator : MonoBehaviour
    {
        private SpriteRenderer _indicatorSpriteRenderer;
        
        private Vector3 _indicatorStartPosition;
        private Vector3 _indicatorEndPosition;    
        
        private void Awake()
        {
            _indicatorSpriteRenderer = GetComponent<SpriteRenderer>();
            _indicatorStartPosition = _indicatorSpriteRenderer.transform.localPosition;
            _indicatorEndPosition = new Vector3(_indicatorStartPosition.x + .05f, _indicatorStartPosition.y + .05f, _indicatorStartPosition.z);
        }

        private void OnEnable()
        {
            StartCoroutine(nameof(AnimateRenderer));
        }

        private void OnDisable()
        {
            StopCoroutine(nameof(AnimateRenderer));
        }
        
        private IEnumerator AnimateRenderer()
        {
            yield return new WaitForSeconds(.5f);
            _indicatorSpriteRenderer.transform.localPosition = _indicatorEndPosition;
            yield return new WaitForSeconds(.5f);
            _indicatorSpriteRenderer.transform.localPosition = _indicatorStartPosition;
            StartCoroutine("AnimateRenderer");
        }
    }
}