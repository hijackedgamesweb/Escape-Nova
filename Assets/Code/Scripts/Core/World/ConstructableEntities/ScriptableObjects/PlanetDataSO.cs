using Code.Scripts.Core.Systems.Resources;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New PlanetDataSO", menuName = "Core/World/ConstructableEntities/PlanetDataSO")]
    public class PlanetDataSO : ScriptableObject
    {
        public string planetName;
        public Sprite sprite;
        public float size;
        public ResourceType primaryResource;
    }
}