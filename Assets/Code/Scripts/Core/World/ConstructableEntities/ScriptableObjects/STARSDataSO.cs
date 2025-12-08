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
        
        [Header("Visual & Orbit")]
        public Sprite sprite;
        public Material material;
        public float orbitDistance = 45f;
        public float orbitSpeed = 5f;
        
        [SerializeReference, SubclassSelector] public List<Upgrade> upgrades;
    }
}