using System;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_UnlockMission : AbstractResearchReward
    {
        public string missionId;

        public override void ApplyReward()
        {
            //hay que conectar con sistema de misiones
            Debug.Log($"Recompensa: Mission unlocked: {missionId}");
        }
        
        public override string GetDescription() => $"Desbloquear Misión: {missionId}";
    }
}