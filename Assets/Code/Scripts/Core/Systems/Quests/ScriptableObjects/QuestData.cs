using System.Collections.Generic;
using Code.Scripts.Core.Systems.Quests.Objectives;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.ScriptableObjects
{
    [System.Serializable]
    public class QuestReward
    {
        public string Description;
        public int Amount;
    }

    [CreateAssetMenu(menuName = "Quest System/Quest Data")]
    public class QuestData : ScriptableObject
    {
        public string QuestId;
        public string Title;
        [TextArea] public string Description;
        [SerializeReference, SubclassSelector] public List<QuestObjective> Objectives;

        public List<QuestReward> Rewards; 
    }
}