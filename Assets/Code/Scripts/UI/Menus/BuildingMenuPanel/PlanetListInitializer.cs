using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems.Planets;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class PlanetListInitializer : MonoBehaviour
    {
        [SerializeField] private List<PlanetDataSO> _planetDataSOs; 
        [SerializeField] private PlanetListPrefab _planetListPrefab;
        
        private PlanetListPrefab _currentPlanetItem;
        private PlanetUnlockManager _unlockManager;
        
        private Dictionary<string, PlanetListPrefab> _createdPlanets = new Dictionary<string, PlanetListPrefab>();

        private void Start()
        {
            try
            {
                _unlockManager = ServiceLocator.GetService<PlanetUnlockManager>();
            }
            catch (System.Exception e)
            {
                return;
            }

            foreach (var planetData in _planetDataSOs)
            {
                AddNewPlanet(planetData);
            }
            
            var alreadyUnlockedPlanets = _unlockManager.GetUnlockedPlanets();
            foreach (var planetData in alreadyUnlockedPlanets)
            {
                AddNewPlanet(planetData);
            }

            _unlockManager.OnPlanetAddedToConstructionList += AddNewPlanet;
        }

        private void OnDestroy()
        {
            if (_unlockManager != null)
            {
                _unlockManager.OnPlanetAddedToConstructionList -= AddNewPlanet;
            }
        }

        private void AddNewPlanet(PlanetDataSO obj)
        {
            if (obj == null) return;
            
            if (_createdPlanets.ContainsKey(obj.constructibleName))
            {
                return;
            }
            
            PlanetListPrefab planetItem = Instantiate(_planetListPrefab, transform);
            planetItem.Initialize(obj, this);
            
            _createdPlanets.Add(obj.constructibleName, planetItem);
        }

        public void SetCurrentPlanetItem(PlanetListPrefab planetItem)
        {
            if (_currentPlanetItem != null)
            {
                _currentPlanetItem.IsSelected = false;
                _currentPlanetItem.UpdateVisualState();
            }

            _currentPlanetItem = planetItem;
            _currentPlanetItem.IsSelected = true;
            _currentPlanetItem.UpdateVisualState();
        }
        
        public PlanetDataSO GetCurrentPlanetData()
        {
            if (_currentPlanetItem == null)
                return null;

            return _currentPlanetItem.PlanetData;
        }
    }
}