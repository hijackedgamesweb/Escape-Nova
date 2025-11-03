using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    public abstract class ConstructibleDataSO : ScriptableObject
    {
        public string constructibleName;
        public Sprite sprite;
        
        [Header("Building Data")]
        public int timeToBuild; 
        public ResourceData[] buildCostResources;
        public int[] buildCostAmounts;
    }
}