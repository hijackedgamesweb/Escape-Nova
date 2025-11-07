using System.Linq;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using Fungus;

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
            UpdateOrbit(planet, 1);
        }

        private void OnPlanetRemoved(Planet planet)
        {
            UpdateOrbit(planet, -1);
        }

        private void UpdateOrbit(Planet planet, int modifier)
        {
            int orbitIndex = planet.OrbitIndex;
            int positionInOrbit = planet.PlanetIndex;
            
            var orbit = _solarSystem.Planets[orbitIndex];
            Planet previousPlanet = orbit[positionInOrbit == 0 ? orbit.Count - 1 : positionInOrbit - 1];
            Planet nextPlanet = orbit[positionInOrbit + 1 < orbit.Count ? positionInOrbit + 1 : 0];

            if (previousPlanet != null)
            {
                if (planet.Name == previousPlanet.Name)
                {
                    previousPlanet.IncreaseProductionPerCycleOfAllResources(productionBonusPercent * modifier);
                }
            }

            if (nextPlanet != null)
            {
                if (planet.Name == nextPlanet.Name && nextPlanet != null)
                {
                    nextPlanet.IncreaseProductionPerCycleOfAllResources(productionBonusPercent * modifier);
                }
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
                        planet.IncreaseProductionPerCycleOfAllResources(productionBonusPercent);
                        previousPlanet.IncreaseProductionPerCycleOfAllResources(productionBonusPercent);
                    }
                    previousPlanet = planet;
                }
            }
        }
    }
}