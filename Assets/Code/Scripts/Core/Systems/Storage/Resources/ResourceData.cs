using UnityEngine;

namespace Code.Scripts.Core.Systems.Resources
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "Game/Resource Data")]
    public class ResourceData : ScriptableObject
    {
        public ResourceType Type;
        public string DisplayName;
        public Sprite Icon;
        public Color Color;
        public int MaxStack = 9999;
        public GameObject VisualPrefab; // Si necesitamos mostrar el prefab
    }
}