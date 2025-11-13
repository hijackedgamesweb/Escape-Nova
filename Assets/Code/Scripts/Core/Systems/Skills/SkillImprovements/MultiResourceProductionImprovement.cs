using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Skills.SkillImprovements
{
    [System.Serializable]
    public class MultiResourceProductionImprovement : SkillImprovement
    {
        public float productionBonusPercent;
        private SolarSystem _solarSystem;

        public override void ApplyImprovement()
        {
            _solarSystem = ServiceLocator.GetService<SolarSystem>();

            if (_solarSystem == null)
            {
                Debug.LogError("MultiResourceProductionImprovement: SolarSystem not found");
                return;
            }

            ApplyImprovementToMultiResourcePlanets();
            Debug.Log($"Applied multi resource production improvement: {productionBonusPercent}%");
        }

        private void ApplyImprovementToMultiResourcePlanets()
        {
            int improvedPlanets = 0;

            foreach (var orbit in _solarSystem.Planets)
            {
                foreach (var planet in orbit)
                {
                    if (planet != null && IsMultiResourcePlanet(planet))
                    {
                        planet.AddImprovement("MultiResourceProduction", productionBonusPercent);
                        improvedPlanets++;
                    }
                }
            }

            Debug.Log($"Improved {improvedPlanets} multi-resource planets");
        }

        private bool IsMultiResourcePlanet(Planet planet)
        {
            if (planet.ResourcePerCycle == null) return false;

            int nonZeroResources = 0;
            foreach (var resourceAmount in planet.ResourcePerCycle)
            {
                if (resourceAmount > 0)
                {
                    nonZeroResources++;
                    if (nonZeroResources > 1) return true;
                }
            }
            return false;
        }
    }
}