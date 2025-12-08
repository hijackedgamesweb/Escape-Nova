using UnityEngine;
using Code.Scripts.Core.Systems.Astrarium; // Namespace de la interfaz

namespace Code.Scripts.Core.Systems.Resources
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "Game/Resource Data")]
    public class ResourceData : ScriptableObject, IAstrariumEntry
    {
        public ResourceType Type;
        public string DisplayName;
        public Sprite Icon;
        public Color Color;
        public int MaxStack = 9999;
        public GameObject VisualPrefab;

        [Header("Astrarium Data")]
        [TextArea(3, 10)] public string astrariumDescription; //lore

        public string GetAstrariumID() => $"resource_{Type.ToString().ToLower()}";
        public string GetDisplayName() => DisplayName;
        public string GetDescription() => astrariumDescription;
        public Sprite GetIcon() => Icon;
        public AstrariumCategory GetCategory() => AstrariumCategory.Resource;
        
        // Como es 2D, devolvemos null. La UI usará el Icon.
        public GameObject Get3DModel() => null; 
    }
}