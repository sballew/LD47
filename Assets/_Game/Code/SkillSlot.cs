using System;
using UnityEngine;

namespace TwinPixels.LD47
{
    public class SkillSlot : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _indicatorSpriteRenderer;

        private bool _indicatorShown = false;

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