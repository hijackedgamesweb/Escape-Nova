using System;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Quests.Rewards
{
    [Serializable]
    public class GiveResourceReward : QuestReward
    {
        public ResourceType resource;
        public int amount;

        public override void ApplyReward()
        {
            StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
            if (storage != null)
            {
                storage.AddResource(resource, amount);
            }
        }
        
        public override string GetRewardInfo()
        {
            return $"{Description}: {amount}";
        }
    }
}