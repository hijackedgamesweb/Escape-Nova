using System;
using Code.Scripts.Core.Systems.Skills;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades
{
    [Serializable]
    public class LocalSynergyUpgrade : Upgrade
    {
        public string baseImprovementType;
        public float basePercentage;
        
        public string synergySkillId;
        public string synergyImprovementType;
        public float synergyPercentage;

        public override void ApplyUpgrade(Planet planet)
        {
            if (planet == null) return;

            planet.AddImprovement(baseImprovementType, basePercentage);
            
            if (SkillImprovementTracker.IsSkillPurchased(synergySkillId))
            {
                planet.AddImprovement(synergyImprovementType, synergyPercentage);
            }
        }
    }
}