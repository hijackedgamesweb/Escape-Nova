using System;
using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_UnlockTechnology : AbstractResearchReward
    {
        public string researchIdToUnlock;

        public override void ApplyReward()
        {
            var researchSystem = ServiceLocator.GetService<ResearchSystem>();
            if (researchSystem != null)
            {
                researchSystem.UnlockResearch(researchIdToUnlock);
                Debug.Log($"Recompensa: Desbloqueando tecnología: {researchIdToUnlock}");
            }
        }
        
        public override string GetDescription() => $"Desbloquear Tecnología: {researchIdToUnlock}";
    }
}