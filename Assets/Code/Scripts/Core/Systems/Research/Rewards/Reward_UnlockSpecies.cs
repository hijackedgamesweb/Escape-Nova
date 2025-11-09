using System;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_UnlockSpecies : AbstractResearchReward
    {
        public string speciesId;

        public override void ApplyReward()
        {
            //hay que conectar con el sistema de especies
            Debug.Log($"Recompensa: Especies desbloqueadas: {speciesId}");
        }
        
        public override string GetDescription() => $"Desbloquear Especie: {speciesId}";
    }
}