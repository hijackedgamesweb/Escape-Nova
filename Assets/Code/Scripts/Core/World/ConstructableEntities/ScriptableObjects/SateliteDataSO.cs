using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New SateliteDataSO", menuName = "Core/World/ConstructableEntities/SateliteDataSO")]
    public class SateliteDataSO : ConstructibleDataSO
    {
        public float size;
        public string desc;
        
    }
}