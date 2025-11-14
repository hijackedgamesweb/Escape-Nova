using System.Linq;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Skills.SkillImprovements
{
    [System.Serializable]
    public class ProductionOnSamePlanetTogetherImprovement : SkillImprovement
    {
        public float productionBonusPercent;
        private SolarSystem _solarSystem;

        public override void ApplyImprovement()
        {
            _solarSystem = ServiceLocator.GetService<SolarSystem>();

            ImproveProductionOnSamePlanetTogether();

            ConstructionEvents.OnPlanetAdded += OnPlanetAdded;
            ConstructionEvents.OnPlanetRemoved += OnPlanetRemoved;
        }

        private void OnPlanetAdded(Planet planet)
        {
            UpdateAdjacentPlanets(planet, 1);
        }

        private void OnPlanetRemoved(Planet planet)
        {
            UpdateAdjacentPlanets(planet, -1);
        }

        private void UpdateAdjacentPlanets(Planet planet, int modifier)
        {
            int orbitIndex = planet.OrbitIndex;
            int positionInOrbit = planet.PlanetIndex;

            var orbit = _solarSystem.Planets[orbitIndex];

            // Planeta anterior
            Planet previousPlanet = orbit[positionInOrbit == 0 ? orbit.Count - 1 : positionInOrbit - 1];
            if (previousPlanet != null && planet.Name == previousPlanet.Name)
            {
                float improvementAmount = productionBonusPercent * modifier;
                previousPlanet.AddImprovement("AdjacentProduction", improvementAmount);
            }

            // Planeta siguiente
            Planet nextPlanet = orbit[positionInOrbit + 1 < orbit.Count ? positionInOrbit + 1 : 0];
            if (nextPlanet != null && planet.Name == nextPlanet.Name)
            {
                float improvementAmount = productionBonusPercent * modifier;
                nextPlanet.AddImprovement("AdjacentProduction", improvementAmount);
            }
        }

        private void ImproveProductionOnSamePlanetTogether()
        {
            foreach (var orbit in _solarSystem.Planets)
            {
                Planet previousPlanet = orbit.Last();
                foreach (var planet in orbit)
                {
                    if (planet == null || previousPlanet == null)
                    {
                        previousPlanet = planet;
                        continue;
                    }

                    if (planet.Name == previousPlanet.Name)
                    {
                        // Aplicar la mejora de adyacencia a ambos planetas
                        planet.AddImprovement("AdjacentProduction", productionBonusPercent);
                        previousPlanet.AddImprovement("AdjacentProduction", productionBonusPercent);
                    }

                    previousPlanet = planet;
                }
            }
        }
    }
}