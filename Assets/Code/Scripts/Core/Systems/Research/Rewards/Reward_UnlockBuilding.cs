using System;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_UnlockBuilding : AbstractResearchReward
    {
        public string buildingId;

        public override void ApplyReward()
        {
            //hay qeu conectarlo con el sistema de construccion
            Debug.Log($"Recompensa: Planet unlocked: {buildingId}");
            //var buildingSystem = ServiceLocator.GetService<BuildingSystem>();
            //buildingSystem.UnlockBuilding(buildingId);
        }
        
        public override string GetDescription() => $"Desbloquear planeta: {buildingId}";
    }
}