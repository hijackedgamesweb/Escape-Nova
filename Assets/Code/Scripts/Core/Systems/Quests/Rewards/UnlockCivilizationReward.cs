using System;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Quests.Rewards
{
    [Serializable]
    public class UnlockCivilizationReward : QuestReward
    {
        public string civilizationName;

        public override void ApplyReward()
        {
            if (string.IsNullOrEmpty(civilizationName))
            {
                return;
            }

            SystemEvents.UnlockDiplomacyPanel();
            WorldManager.Instance.AddCivilization(civilizationName); 
        }

        public override string GetRewardInfo()
        {
            return Description;
        }
    }
}