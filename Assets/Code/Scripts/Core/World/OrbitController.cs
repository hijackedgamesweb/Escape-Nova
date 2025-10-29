using Code.Scripts.Core.World.ConstructableEntities;
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

        public void Initialize(Planet planet, float radius, int index, int total, float speed)
        {
            _planet = planet;
            orbitRadius = radius;
            _index = index;
            _totalPlanets = total;
            rotationSpeed = speed;
        }

        void Update()
        {
            if (_planet == null) return;

            float angle = (Time.time * rotationSpeed) + _index * (360f / _totalPlanets);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 newPos = new Vector3(orbitRadius * Mathf.Cos(rad), orbitRadius * Mathf.Sin(rad), _planet.transform.position.z);
            _planet.transform.localPosition = newPos;
        }
    }
}