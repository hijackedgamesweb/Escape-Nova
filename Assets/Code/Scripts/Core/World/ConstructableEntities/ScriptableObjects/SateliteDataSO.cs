using System.Collections.Generic;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New SateliteDataSO", menuName = "Core/World/ConstructableEntities/SateliteDataSO")]
    public class SateliteDataSO : ConstructibleDataSO, IAstrariumEntry
    {
        public float size;
        [TextArea] public string desc;
        public Sprite sateliteIcon; 

        [SerializeReference, SubclassSelector] public List<Upgrade> upgrades;

        public string GetAstrariumID() => $"sat_{constructibleName.Trim().ToLower()}";
        public string GetDisplayName() => constructibleName;
        public string GetDescription() => desc;
        
        public Sprite GetIcon() => sateliteIcon != null ? sateliteIcon : sprite; 
        
        public AstrariumCategory GetCategory() => AstrariumCategory.Satellite;
        public GameObject Get3DModel() => null;
    }
}