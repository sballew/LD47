using System;
using System.Collections;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class SkillSlot : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _indicatorSpriteRenderer;
        
        [SerializeField]
        private SpriteRenderer _fillRenderer;

        private bool _indicatorShown = false;

        private Vector3 _indicatorStartPosition;
        private Vector3 _indicatorEndPosition;

        private void Start()
        {
            _indicatorStartPosition = _indicatorSpriteRenderer.transform.localPosition;
            _indicatorEndPosition = new Vector3(_indicatorStartPosition.x + .05f, _indicatorStartPosition.y + .05f, _indicatorStartPosition.z);
            StartCoroutine("AnimateRenderer");
        }

        private void Update()
        {
            if (GameManager.Instance.isPlayerCarryingGem && !_indicatorShown)
            {
                ShowIndicator();
            }
            else if (_indicatorShown && !GameManager.Instance.isPlayerCarryingGem)
            {
                HideIndicator();
            }
        }

        public void SetSlotFill(bool fill)
        {
            _fillRenderer.enabled = fill;
        }

        private IEnumerator AnimateRenderer()
        {
            yield return new WaitForSeconds(.5f);
            _indicatorSpriteRenderer.transform.localPosition = _indicatorEndPosition;
            yield return new WaitForSeconds(.5f);
            _indicatorSpriteRenderer.transform.localPosition = _indicatorStartPosition;
            StartCoroutine("AnimateRenderer");
        }

        private void ShowIndicator()
        {
            _indicatorSpriteRenderer.enabled = true;
        }

        private void HideIndicator()
        {
            _indicatorSpriteRenderer.enabled = false;
        }
    }
}