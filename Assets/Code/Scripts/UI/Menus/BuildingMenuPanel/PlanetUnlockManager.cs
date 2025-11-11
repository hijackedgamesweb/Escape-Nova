using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Planets
{
    public class PlanetUnlockManager : MonoBehaviour
    {
        private List<PlanetDataSO> _unlockedPlanets = new List<PlanetDataSO>();

        public event System.Action<PlanetDataSO> OnPlanetAddedToConstructionList;

        private void Awake()
        {
            ServiceLocator.RegisterService(this);
            ResearchEvents.OnNewPlanetResearched += HandleNewPlanetResearched;
        }

        private void OnDestroy()
        {
            ResearchEvents.OnNewPlanetResearched -= HandleNewPlanetResearched;
        }

        private void HandleNewPlanetResearched(PlanetDataSO planet)
        {
            if (planet == null || _unlockedPlanets.Contains(planet))
            {
                return;
            }

            _unlockedPlanets.Add(planet);
            
            OnPlanetAddedToConstructionList?.Invoke(planet);
        }

        public List<PlanetDataSO> GetUnlockedPlanets()
        {
            return _unlockedPlanets;
        }
    }
}