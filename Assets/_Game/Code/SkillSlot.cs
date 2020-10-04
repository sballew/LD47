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

        [SerializeField]
        private SkillSlotType _skillSlotType;

        public bool startFilled = false;
        public bool acceptsGem = true;
        
        private bool _indicatorShown = false;

        private Vector3 _indicatorStartPosition;
        private Vector3 _indicatorEndPosition;

        private void Start()
        {
            _indicatorStartPosition = _indicatorSpriteRenderer.transform.localPosition;
            _indicatorEndPosition = new Vector3(_indicatorStartPosition.x + .05f, _indicatorStartPosition.y + .05f, _indicatorStartPosition.z);

            if (acceptsGem)
            {
                StartCoroutine("AnimateRenderer");
            }
            
            SetSlotFill(startFilled);
        }

        private void Update()
        {
            if (acceptsGem && GameManager.Instance.isPlayerCarryingGem && !_indicatorShown)
            {
                ShowIndicator();
            }
            else if (acceptsGem && _indicatorShown && !GameManager.Instance.isPlayerCarryingGem)
            {
                HideIndicator();
            }
        }

        public void SetSlotFill(bool fill)
        {
            _fillRenderer.enabled = fill;
            switch (_skillSlotType)
            {
                case SkillSlotType.Boots:
                    GameManager.Instance.player.Motor.SpeedBootsEnabled = fill;
                    break;
            }
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