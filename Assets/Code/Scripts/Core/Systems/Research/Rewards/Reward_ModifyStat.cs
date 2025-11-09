using System;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_ModifyStat : AbstractResearchReward
    {
        public string statName;
        public float value;

        public override void ApplyReward()
        {
            //para modificar las estadisticas del jugador
            Debug.Log($"Recompensa: Stats modificadas: {statName} by {value}");
            //var playerStats = ServiceLocator.GetService<PlayerStats>();
            //playerStats.ModifyStat(statName, value);
        }
        
        public override string GetDescription() => $"Modificar stat: {statName} en {value}";
    }
}