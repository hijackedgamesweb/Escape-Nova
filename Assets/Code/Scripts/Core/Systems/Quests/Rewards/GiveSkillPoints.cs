using System;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Core.Systems.Skills;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.Rewards
{
    [Serializable]
    public class GiveSkillPoints : QuestReward
    {
        public int points;
        
        public override void ApplyReward()
        {
            ServiceLocator.GetService<SkillTreeManager>().AddSkillPoints(points);
        }

        public override string GetRewardInfo()
        {
            return Description;
        }
    }
}