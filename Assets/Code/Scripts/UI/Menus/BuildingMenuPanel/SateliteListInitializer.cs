using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems.Storage;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class SateliteListInitializer : MonoBehaviour
    {
        [SerializeField] private List<SateliteDataSO> _sateliteDataSOs;
        [SerializeField] private SateliteListPrefab _sateliteListPrefab;
        
        private SateliteListPrefab _currentPlanetItem;
        private StorageSystem _storageSystem;

        private void Start()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();

            foreach (var sateliteData in _sateliteDataSOs)
            {
                SateliteListPrefab sateliteItem = Instantiate(_sateliteListPrefab, transform);
                sateliteItem.Initialize(sateliteData, this);
            }
            ResearchEvents.OnNewSateliteResearched += AddNewSatelite;
        }
        public void SetCurrentSateliteItem(SateliteListPrefab satelite)
        {
            if (_currentPlanetItem != null)
            {
                _currentPlanetItem.IsSelected = false;
                _currentPlanetItem.UpdateVisualState();
            }

            _currentPlanetItem = satelite;
            _currentPlanetItem.IsSelected = true;
            _currentPlanetItem.UpdateVisualState();
        }
        public SateliteDataSO GetCurrentSateliteData()
        {
            if (_currentPlanetItem == null)
                return null;

            return _currentPlanetItem.SateliteData;
        }
        public void AddNewSatelite(SateliteDataSO sateliteData)
        {
            SateliteListPrefab sateliteItem = Instantiate(_sateliteListPrefab, transform);
            sateliteItem.Initialize(sateliteData, this);
            _sateliteDataSOs.Add(sateliteData);
        }
        public bool ConsumeResourcesForSatelite(SateliteDataSO sateliteData)
        {
            if (sateliteData == null) return false;
            for (int i = 0; i < sateliteData.buildCostAmounts.Length; i++)
            {
                if (!_storageSystem.HasResource(sateliteData.buildCostResources[i].Type, sateliteData.buildCostAmounts[i]))
                {
                    return false;
                }
            }
            for (int i = 0; i < sateliteData.buildCostAmounts.Length; i++)
            {
                _storageSystem.ConsumeResource(
                    sateliteData.buildCostResources[i].Type, 
                    sateliteData.buildCostAmounts[i]
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