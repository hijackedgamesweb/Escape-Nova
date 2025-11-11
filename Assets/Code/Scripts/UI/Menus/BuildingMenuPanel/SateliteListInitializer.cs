using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class SateliteListInitializer : MonoBehaviour
    {
        [SerializeField] private List<SateliteDataSO> _sateliteDataSOs;
        [SerializeField] private SateliteListPrefab _sateliteListPrefab;
        
        private SateliteListPrefab _currentPlanetItem;
        private void Start()
        {
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
    }
}