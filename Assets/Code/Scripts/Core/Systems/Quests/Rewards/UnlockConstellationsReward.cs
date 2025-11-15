using System;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;

namespace Code.Scripts.Core.Systems.Quests.Rewards
{
    [Serializable]
    public class UnlockConstellationsReward : QuestReward
    {
        public override void ApplyReward()
        {
            SystemEvents.UnlockConstellations();
        }

        public override string GetRewardInfo()
        {
            return Description;
        }
    }
}