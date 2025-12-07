using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Environment
{
    public class SunColorController : MonoBehaviour
    {
        [Header("Configuración de Color")]
        [SerializeField] private Color startColor = Color.yellow;
        [SerializeField] private Color endColor = Color.blue;
        
        [Tooltip("Un valor > 1 hará que el cambio sea más lento al principio y rápido al final.")]
        [SerializeField] private float colorChangeExponent = 3.0f; 

        private Material sunRenderer;

        private IGameTime _gameTime;
        private float _maxCycles;

        private void Awake()
        {
            if (sunRenderer == null)
            {
                sunRenderer = GetComponent<Material>();
            }
        }

        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _maxCycles = ServiceLocator.GetService<GameTimeManager>().maxCycles;

            _gameTime.OnCycleCompleted += UpdateSunColor;

            SetInitialColor();
        }

        private void OnDestroy()
        {
            if (_gameTime != null)
            {
                _gameTime.OnCycleCompleted -= UpdateSunColor;
            }
        }

        private void SetInitialColor()
        {
            if (sunRenderer != null)
            {
                sunRenderer.SetColor("TargetColor", startColor);
            }
        }

        private void UpdateSunColor(int currentCycle)
        {
            if (sunRenderer == null || _maxCycles <= 0) return;

            float t_linear = currentCycle / _maxCycles;
            
            float t_curved = Mathf.Pow(t_linear, colorChangeExponent);

            sunRenderer.SetColor("TargetColor", Color.Lerp(startColor, endColor, t_curved));
        }
    }
}