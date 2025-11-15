using System;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Quests.Rewards
{
    [Serializable]
    public class UnlockResearchReward : QuestReward
    {
        public string researchIdToUnlock;

        public override void ApplyReward()
        {
            if (string.IsNullOrEmpty(researchIdToUnlock))
            {
                return;
            }

            ResearchSystem researchSystem = ServiceLocator.GetService<ResearchSystem>();

            if (researchSystem != null)
            {
                researchSystem.UnlockResearch(researchIdToUnlock);
            }
        }

        public override string GetRewardInfo()
        {
            return Description;
        }
    }
}