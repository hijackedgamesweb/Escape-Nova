using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.Patterns.Factory
{
    public class PlanetFactory : MonoBehaviour
    {
        [SerializeField] private GameObject _planetPrefab;
        
        public Planet CreatePlanet(Vector3 position, PlanetDataSO data, Transform parent)
        {
            GameObject planetObject = Object.Instantiate(_planetPrefab, position, Quaternion.identity, parent);
            Planet planet = planetObject.GetComponent<Planet>();
            planet.InitializePlanet(data);
            return planet;
        }
    }
}