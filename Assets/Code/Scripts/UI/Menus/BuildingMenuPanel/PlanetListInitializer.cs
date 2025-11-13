using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems.Planets;
using Code.Scripts.Core.Systems.Storage;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class PlanetListInitializer : MonoBehaviour
    {
        [SerializeField] private List<PlanetDataSO> _planetDataSOs; 
        [SerializeField] private PlanetListPrefab _planetListPrefab;
        
        private PlanetListPrefab _currentPlanetItem;
        private PlanetUnlockManager _unlockManager;
        
        private StorageSystem _storageSystem;
        private Dictionary<string, PlanetListPrefab> _createdPlanets = new Dictionary<string, PlanetListPrefab>();

        private void Start()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            
            try
            {
                _unlockManager = ServiceLocator.GetService<PlanetUnlockManager>();
            }
            catch (System.Exception e)
            {
            }

            foreach (var planetData in _planetDataSOs)
            {
                AddNewPlanet(planetData);
            }
            
            if (_unlockManager != null)
            {
                var alreadyUnlockedPlanets = _unlockManager.GetUnlockedPlanets();
                foreach (var planetData in alreadyUnlockedPlanets)
                {
                    AddNewPlanet(planetData);
                }
                _unlockManager.OnPlanetAddedToConstructionList += AddNewPlanet;
            }
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
        public bool ConsumeResourcesForPlanet(PlanetDataSO planetData)
        {
            if (planetData == null) return false;
            for (int i = 0; i < planetData.buildCostAmounts.Length; i++)
            {
                if (!_storageSystem.HasResource(planetData.buildCostResources[i].Type, planetData.buildCostAmounts[i]))
                {
                    return false;
                }
            }
            for (int i = 0; i < planetData.buildCostAmounts.Length; i++)
            {
                _storageSystem.ConsumeResource(
                    planetData.buildCostResources[i].Type, 
                    planetData.buildCostAmounts[i]
                );
            }
            return true;
        }
        public void ClearSelection()
        {
            if (_currentPlanetItem != null)
            {
                _currentPlanetItem.IsSelected = false;
                _currentPlanetItem.UpdateVisualState();
                _currentPlanetItem = null;
            }
        }
    }
}