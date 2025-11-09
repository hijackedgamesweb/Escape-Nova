// ResearchNode.cs
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Research.Rewards;
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
        public int researchTimeInSeconds = 60;
        
        [Header("Cost")]
        public List<ResearchCost> resourceCosts;
        
        [Header("Rewards")]
        [SerializeReference, SubclassSelector]
        public List<AbstractResearchReward> rewards = new List<AbstractResearchReward>();
        
        [Header("UI Settings")]
        public int tier = 1;
        public Vector2 treePosition;
        
        [Header("Unlocks")]
        public List<string> unlocksResearchIds;
    }
}