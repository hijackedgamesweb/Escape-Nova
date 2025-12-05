using UnityEngine;
using UnityEngine.UI;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.UI.World
{
    public class PlanetConstructionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private GameObject _uiContainer;

        [Header("Settings")]
        [SerializeField] private bool _hideOnComplete = true;

        private Planet _planet;

        private void Awake()
        {
            _planet = GetComponentInParent<Planet>();
            
            if (_progressSlider == null)
            {
                return;
            }
            if (_uiContainer != null) _uiContainer.SetActive(false);
        }

        private void OnEnable()
        {
            if (_planet != null)
            {
                _planet.OnConstructionProgress += UpdateProgress;
                _planet.OnConstructionCompleted += HandleComplete;
            }
        }

        private void OnDisable()
        {
            if (_planet != null)
            {
                _planet.OnConstructionProgress -= UpdateProgress;
                _planet.OnConstructionCompleted -= HandleComplete;
            }
        }

        private void UpdateProgress(float progress)
        {
            if (_uiContainer != null && !_uiContainer.activeSelf)
            {
                _uiContainer.SetActive(true);
            }

            if (_progressSlider != null)
            {
                _progressSlider.value = progress;
            }
        }

        private void HandleComplete()
        {
            if (_hideOnComplete && _uiContainer != null)
            {
                _uiContainer.SetActive(false);
            }
        }
    }
}