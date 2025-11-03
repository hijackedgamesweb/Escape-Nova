using System;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.World
{
    public class OrbitController : MonoBehaviour
    {
        public float orbitRadius;
        public float rotationSpeed;
        private Planet _planet;
        private int _index;
        private int _totalPlanets;
        private int _orbitIndex;
        
        private IGameTime _gameTime;

        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnTimeAdvanced += UpdatePlanets;
        }

        public void Initialize(Planet planet, float radius, int index, int total, float speed, int orbitIndex)
        {
            _planet = planet;
            orbitRadius = radius;
            _index = index;
            _totalPlanets = total;
            rotationSpeed = speed;
            _orbitIndex = orbitIndex;
        }

        void UpdatePlanets(float deltaTime)
        {
            if (_planet == null) return;
            
            var dir = _orbitIndex % 2 == 0 ? 1 : -1;
            float angle = (_gameTime.GameTime * (rotationSpeed * dir) / (_orbitIndex + 1)) + _index * (360f / _totalPlanets) ;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 newPos = new Vector3(orbitRadius * Mathf.Cos(rad), orbitRadius * Mathf.Sin(rad), _planet.transform.position.z);
            _planet.transform.localPosition = newPos;
        }
    }
}