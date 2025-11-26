using UnityEngine;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.World
{
    public class SatelliteOrbitController : MonoBehaviour
    {
        private float _orbitRadius;
        private float _rotationSpeed;
        private float _slotAngleOffset; 
        
        private IGameTime _gameTime;
        private bool _isInitialized = false;

        public void Initialize(float radius, float speed, float slotFixedAngle)
        {
            _orbitRadius = radius;
            _rotationSpeed = speed;
            _slotAngleOffset = slotFixedAngle;

            try
            {
                _gameTime = ServiceLocator.GetService<IGameTime>();
                
                if (_gameTime != null)
                {
                    _isInitialized = true;
                }
            }
            catch (System.Exception)
            {
            }
            
            UpdatePosition();
        }

        private void Update()
        {
            if (!_isInitialized) return;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_orbitRadius <= 0.1f) _orbitRadius = 1.5f;

            float timeFactor = _gameTime.GameTime * _rotationSpeed;
            
            float totalAngle = timeFactor + _slotAngleOffset;
            
            float rad = totalAngle * Mathf.Deg2Rad;
            
            Vector3 newPos = new Vector3(
                Mathf.Cos(rad) * _orbitRadius,
                Mathf.Sin(rad) * _orbitRadius,
                -0.1f 
            );
            
            transform.localPosition = newPos;
        }
    }
}