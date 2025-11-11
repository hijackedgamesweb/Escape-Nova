using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class PlanetListInitializer : MonoBehaviour
    {
        [SerializeField] private List<PlanetDataSO> _planetDataSOs;
        [SerializeField] private PlanetListPrefab _planetListPrefab;
        
        private PlanetListPrefab _currentPlanetItem;
        
        private void Start()
        {
            foreach (var planetData in _planetDataSOs)
            {
                PlanetListPrefab planetItem = Instantiate(_planetListPrefab, transform);
                planetItem.Initialize(planetData, this);
            }
            
            ResearchEvents.OnNewPlanetResearched += AddNewPlanet;
        }

        private void AddNewPlanet(PlanetDataSO obj)
        {
            PlanetListPrefab planetItem = Instantiate(_planetListPrefab, transform);
            planetItem.Initialize(obj, this);
            _planetDataSOs.Add(obj);
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