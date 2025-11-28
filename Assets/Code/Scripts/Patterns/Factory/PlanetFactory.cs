using Code.Scripts.Animation;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.Patterns.Factory
{
    public class PlanetFactory : MonoBehaviour
    {
        [Header("Prefab Reference")]
        [SerializeField] private GameObject _planetPrefab;
        
        public Planet CreatePlanet(Vector3 position, PlanetDataSO data, Transform parent, int orbitIndex, int positionInOrbit)
        {
            if (_planetPrefab == null)
            {
                return null;
            }
            if (!_planetPrefab) 
            {
                return null;
            }
            GameObject planetObject = Object.Instantiate(_planetPrefab, position, Quaternion.identity, parent);
            var animator = planetObject.GetComponent<SpriteAnimatorController>();
            if (animator != null)
            {
                animator.LoadAnimation(data);
            }
            else
            {
                Debug.LogWarning($"[PlanetFactory] El prefab del planeta no tiene 'SpriteAnimatorController'.");
            }
            Planet planet = planetObject.GetComponent<Planet>();
            if (planet != null)
            {
                planet.InitializePlanet(data, orbitIndex, positionInOrbit);
                planet.name = $"{data.constructibleName}_{orbitIndex}_{positionInOrbit}";
            }
            else
            {
                Debug.LogError($"[PlanetFactory] El prefab instanciado no tiene el componente 'Planet'.");
            }

            return planet;
        }
    }
}