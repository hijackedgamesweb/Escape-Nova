using System.Collections.Generic;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Resources;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New PlanetDataSO", menuName = "Core/World/ConstructableEntities/PlanetDataSO")]
    public class PlanetDataSO : ConstructibleDataSO, IAstrariumEntry
    {
        [Header("Astrarium Data")]
        [TextArea(3, 10)] public string loreDescription;
        
        [Header("Planet Stats")]
        public float size;
        
        [Header("Production Data")]
        public List<ResourceData> producibleResources;
        public int[] resourcePerCycle;
        
        [Header("Animation")]
        public Sprite[] animationFrames;
        public float frameRate = 0.1f;
        
        public int maxSatellites;

        public string GetAstrariumID() => $"planet_{constructibleName.Trim().ToLower()}";
        public string GetDisplayName() => constructibleName;
        public string GetDescription() => loreDescription;
        public Sprite GetIcon() => sprite;
        public AstrariumCategory GetCategory() => AstrariumCategory.Planet;
        public GameObject Get3DModel() => null;
    }
}