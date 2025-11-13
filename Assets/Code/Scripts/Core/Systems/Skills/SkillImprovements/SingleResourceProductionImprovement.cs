using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Skills.SkillImprovements
{
    [System.Serializable]
    public class SingleResourceProductionImprovement : SkillImprovement
    {
        public float productionBonusPercent;
        private SolarSystem _solarSystem;

        public override void ApplyImprovement()
        {
            _solarSystem = ServiceLocator.GetService<SolarSystem>();

            if (_solarSystem == null)
            {
                Debug.LogError("SingleResourceProductionImprovement: SolarSystem not found");
                return;
            }

            ApplyImprovementToSingleResourcePlanets();
            Debug.Log($"Applied single resource production improvement: {productionBonusPercent}%");
        }

        private void ApplyImprovementToSingleResourcePlanets()
        {
            int improvedPlanets = 0;

            foreach (var orbit in _solarSystem.Planets)
            {
                foreach (var planet in orbit)
                {
                    if (planet != null && IsSingleResourcePlanet(planet))
                    {
                        planet.AddImprovement("SingleResourceProduction", productionBonusPercent);
                        improvedPlanets++;
                    }
                }
            }

            Debug.Log($"Improved {improvedPlanets} single-resource planets");
        }

        private bool IsSingleResourcePlanet(Planet planet)
        {
            if (planet.ResourcePerCycle == null) return false;

            int nonZeroResources = 0;
            foreach (var resourceAmount in planet.ResourcePerCycle)
            {
                if (resourceAmount > 0)
                {
                    nonZeroResources++;
                    if (nonZeroResources > 1) return false;
                }
            }
            return nonZeroResources == 1;
        }
    }
}