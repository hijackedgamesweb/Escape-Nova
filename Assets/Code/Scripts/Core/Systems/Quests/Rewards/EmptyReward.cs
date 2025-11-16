using System;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;

namespace Code.Scripts.Core.Systems.Quests.Rewards
{
    [Serializable]
    public class EmptyReward : QuestReward
    {
        public override void ApplyReward()
        {
        }

        public override string GetRewardInfo()
        {
            return Description;
        }
    }
}