using System;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class RewardUnlockSatelite : AbstractResearchReward
    {
        public SateliteDataSO sateliteData;

        public override void ApplyReward()
        {
            ResearchEvents.OnNewSateliteResearched?.Invoke(sateliteData);
        }
        
        public override string GetDescription() => $"Desbloquear planeta: {sateliteData.constructibleName}";
    }
}