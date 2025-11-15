using System.Collections.Generic;
using Code.Scripts.Core.Systems.Quests.Objectives;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.ScriptableObjects
{
    [System.Serializable]
    public abstract class QuestReward
    {
        [TextArea]
        public string Description;
        
        public abstract void ApplyReward();
        
        public abstract string GetRewardInfo();
    }

    [CreateAssetMenu(menuName = "Quest System/Quest Data")]
    public class QuestData : ScriptableObject
    {
        public string QuestId;
        public string Title;
        [TextArea] public string Description;
        [SerializeReference, SubclassSelector] public List<QuestObjective> Objectives;

        [SerializeReference, SubclassSelector]
        public List<QuestReward> Rewards; 
    }
}