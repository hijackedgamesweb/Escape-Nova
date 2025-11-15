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
        }
    }
}