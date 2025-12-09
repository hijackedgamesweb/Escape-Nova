using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New SateliteDataSO", menuName = "Core/World/ConstructableEntities/SateliteDataSO")]
    public class SateliteDataSO : ConstructibleDataSO
    {
        public float size;
        public string desc;
        [SerializeReference, SubclassSelector] public List<Upgrade> upgrades;
    }
}