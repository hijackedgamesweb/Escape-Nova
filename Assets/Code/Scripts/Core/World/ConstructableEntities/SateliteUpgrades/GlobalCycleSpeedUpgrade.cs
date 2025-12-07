using Code.Scripts.Core.Managers;
using System;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades
{
    [Serializable]
    public class GlobalCycleSpeedUpgrade : Upgrade
    {
        [SerializeField]
        private float globalSpeedBonusPercent = 15.0f;
        
        [SerializeField]
        private string globalImprovementKey = "GlobalCycleSpeed";

        public override void ApplyUpgrade(Planet planet)
        {
            Planet.AddGlobalImprovement(globalImprovementKey, globalSpeedBonusPercent);

            NotificationManager.Instance.CreateNotification(
                $"Global research and crafting speed has improved {globalSpeedBonusPercent}%",
                NotificationType.Info);
        }
    }
}