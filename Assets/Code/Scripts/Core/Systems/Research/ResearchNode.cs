// ResearchNode.cs
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research
{
    [CreateAssetMenu(fileName = "New Research", menuName = "Game/Research/Research Node")]
    public class ResearchNode : ScriptableObject
    {
        [Header("Basic Info")]
        public string researchId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        
        [Header("Requirements")]
        public List<ResearchPrerequisite> prerequisites;
        public int researchTimeInSeconds = 60; // Tiempo en segundos
        
        [Header("Cost")]
        public List<ResearchCost> resourceCosts;
        
        [Header("Rewards")]
        public List<ResearchReward> rewards;
        
        [Header("UI Settings")]
        public int tier = 1; // Para organizar en árbol tecnológico
        public Vector2 treePosition; // Posición en el árbol tecnológico
        
        [Header("Unlocks")]
        public List<string> unlocksResearchIds; // Qué investigaciones desbloquea
    }
}