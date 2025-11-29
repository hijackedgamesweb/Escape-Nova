using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using NUnit.Framework;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New PlanetDataSO", menuName = "Core/World/ConstructableEntities/PlanetDataSO")]
    public class PlanetDataSO : ConstructibleDataSO
    {
        public float size;
        
        [Header("Production Data")]
        public List<ResourceData> producibleResources;
        public int[] resourcePerCycle;
        
        [Header("Animation")]
        public Sprite[] animationFrames;
        public float frameRate = 0.1f;
        
        public int maxSatellites;
    }
}