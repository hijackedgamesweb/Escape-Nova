using System;
using Code.Scripts.Core.Managers;
using UnityEngine;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_IncreaseMaxStack : AbstractResearchReward
    {
        public ResourceType resourceType;
        public int increaseAmount;

        public override void ApplyReward()
        {
            var storage = WorldManager.Instance.Player.StorageSystem;
            if (storage != null)
            {
                Debug.Log($"Recompensa: Max stack aumentado para {resourceType} en {increaseAmount}");
                storage.AddMaxCapacity(resourceType, increaseAmount); 
            }
        }
        
        public override string GetDescription() => $"Aumentar Max Stack de {resourceType} en {increaseAmount}";
    }
}