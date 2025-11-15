using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Skills.SkillImprovements
{
    [System.Serializable]
    public class LocalSynergySkill : SkillImprovement
    {
        public string skillIdToUnlock;
        public SateliteDataSO requiredSatelite;
        public string synergyImprovementType;
        public float synergyPercentage;

        public override void ApplyImprovement()
        {
            if (string.IsNullOrEmpty(skillIdToUnlock)) return;

            SkillImprovementTracker.MarkSkillAsPurchased(skillIdToUnlock);

            Planet[] allPlanets = Object.FindObjectsOfType<Planet>();
            foreach (var planet in allPlanets)
            {
                foreach (var satelite in planet.Satelites)
                {
                    if (satelite.SateliteData == requiredSatelite)
                    {
                        planet.AddImprovement(synergyImprovementType, synergyPercentage);
                    }
                }
            }
        }
    }
}