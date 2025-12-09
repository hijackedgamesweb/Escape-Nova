using System.Collections.Generic;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New STARSDataSO", menuName = "Core/World/ConstructableEntities/STARSDataSO")]
    public class STARSDataSO : ConstructibleDataSO, IAstrariumEntry
    {
        public float size;
        [TextArea] public string desc;
        
        [Header("Visual & Orbit")]
        public Material material;
        public float orbitDistance = 45f;
        public float orbitSpeed = 5f;
        
        [SerializeReference, SubclassSelector] public List<Upgrade> upgrades;

        public string GetAstrariumID() => $"stars_{constructibleName.Trim().ToLower()}";
        public string GetDisplayName() => constructibleName;
        public string GetDescription() => desc;
        public Sprite GetIcon() => sprite;
        public AstrariumCategory GetCategory() => AstrariumCategory.Special;
        public GameObject Get3DModel() => null;
    }
}