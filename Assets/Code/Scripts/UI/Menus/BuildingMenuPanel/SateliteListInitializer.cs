using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.UI.Menus.BuildingMenuPanel.Code.Scripts.Core.Systems.Satelites;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class SateliteListInitializer : MonoBehaviour
    {
        [SerializeField] private List<SateliteDataSO> _sateliteDataSOs;
        [SerializeField] private SateliteListPrefab _sateliteListPrefab;
        
        private SateliteListPrefab _currentSateliteItem;
        private StorageSystem _storageSystem;
        
        private SateliteUnlockManager _unlockManager;
        private Dictionary<string, SateliteListPrefab> _createdSatelites = new Dictionary<string, SateliteListPrefab>();

        private void Start()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            
            try
            {
                _unlockManager = ServiceLocator.GetService<SateliteUnlockManager>();
            }
            catch (System.Exception e)
            {
            }

            foreach (var sateliteData in _sateliteDataSOs)
            {
                AddNewSatelite(sateliteData);
            }
            
            if (_unlockManager != null)
            {
                var alreadyUnlockedSatelites = _unlockManager.GetUnlockedSatelites();
                foreach (var sateliteData in alreadyUnlockedSatelites)
                {
                    AddNewSatelite(sateliteData);
                }
                _unlockManager.OnSateliteAddedToConstructionList += AddNewSatelite;
            }
        }

        private void OnDestroy()
        {
            if (_unlockManager != null)
            {
                _unlockManager.OnSateliteAddedToConstructionList -= AddNewSatelite;
            }
        }
        
        public void SetCurrentSateliteItem(SateliteListPrefab satelite)
        {
            if (_currentSateliteItem != null)
            {
                _currentSateliteItem.IsSelected = false;
                _currentSateliteItem.UpdateVisualState();
            }

            _currentSateliteItem = satelite;
            _currentSateliteItem.IsSelected = true;
            _currentSateliteItem.UpdateVisualState();
        }
        
        public SateliteDataSO GetCurrentSateliteData()
        {
            if (_currentSateliteItem == null)
                return null;

            return _currentSateliteItem.SateliteData;
        }
        
        public void AddNewSatelite(SateliteDataSO sateliteData)
        {
            if (sateliteData == null) return;
            
            if (_createdSatelites.ContainsKey(sateliteData.constructibleName))
            {
                return;
            }
            
            SateliteListPrefab sateliteItem = Instantiate(_sateliteListPrefab, transform);
            sateliteItem.Initialize(sateliteData, this);
            
            _createdSatelites.Add(sateliteData.constructibleName, sateliteItem);
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
            if (_currentSateliteItem != null)
            {
                _currentSateliteItem.IsSelected = false;
                _currentSateliteItem.UpdateVisualState();
                _currentSateliteItem = null;
            }
        }
    }
}