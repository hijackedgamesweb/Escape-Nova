using System;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class PlanetListPrefab : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _planetSprite;
        [SerializeField] private TMP_Text _planetName;
        [SerializeField] private TMP_Text _planetProductionResource;
        [SerializeField] private ResourceCostInitializer _resourceCostInitializer;

        PlanetListInitializer _parentInitializer;
        public bool IsSelected { get; set; }
        public bool IsAffordable { get; set; }
        public PlanetDataSO PlanetData { get; set; }
        private IGameTime _gameTime;
        private StorageSystem _storageSystem;
        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnCycleCompleted += UpdatePrefabAvailability;
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
        }

        private void UpdatePrefabAvailability(int obj)
        {
            bool canAfford = true;
            for (int i = 0; i < PlanetData.buildCostAmounts.Length; i++)
            {
                ResourceType resource = PlanetData.buildCostResources[i].Type;
                var cost = PlanetData.buildCostAmounts[i];
                if (_storageSystem.HasResource(resource, cost))
                {
                    continue;
                }
                else
                {
                    canAfford = false;
                    break;
                }
            }
            _planetSprite.color = canAfford ? Color.white : Color.red;
            IsAffordable = canAfford;
        }

        public void Initialize(PlanetDataSO planetData, PlanetListInitializer parentInitializer)
        {
            PlanetData = planetData;
            _parentInitializer = parentInitializer;
            _planetSprite.sprite = planetData.sprite;
            _planetName.text = planetData.constructibleName;
            
            var production = "";
            for (int i = 0; i < planetData.producibleResources.Count; i++)
            {
                production += $"{planetData.resourcePerCycle[i]} {planetData.producibleResources[i].DisplayName} / cycle";
                if (i < planetData.producibleResources.Count - 1)
                    production += ", \n";
            }
            _planetProductionResource.text = production;
            _resourceCostInitializer.Initialize(planetData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(IsAffordable)
                _parentInitializer.SetCurrentPlanetItem(this);
        }

        public void UpdateVisualState()
        {
            if (IsSelected)
            {
                _planetSprite.color = Color.yellow;
            }
            else
            {
                _planetSprite.color = Color.white;
            }
        }
    }
}