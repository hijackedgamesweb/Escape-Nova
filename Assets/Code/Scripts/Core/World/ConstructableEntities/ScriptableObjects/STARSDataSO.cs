using System.Collections.Generic;
using Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New STARSDataSO", menuName = "Core/World/ConstructableEntities/STARSDataSO")]
    public class STARSDataSO : ConstructibleDataSO
    {
        public float size;
        public string desc;
        [SerializeReference, SubclassSelector] public List<Upgrade> upgrades;
    }
}