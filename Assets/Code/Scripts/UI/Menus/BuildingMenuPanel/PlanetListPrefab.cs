using System;
using Code.Scripts.Core.Managers;
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
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _availableColor = Color.red;
        [SerializeField] private Color _unavailableColor = Color.gray;

        PlanetListInitializer _parentInitializer;
        public bool IsSelected { get; set; }
        public bool IsAffordable { get; set; }
        public PlanetDataSO PlanetData { get; set; }
        private IGameTime _gameTime;
        private StorageSystem _storageSystem;
        
        private int[] _costPerResource;
        private float totalDiscount;
        
        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnCycleCompleted += UpdatePrefabAvailability;
        }

        private void UpdatePrefabAvailability(int obj = 0)
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

            if (!IsSelected)
            {
                GetComponent<Image>().color = canAfford ? _availableColor : _unavailableColor;
                IsAffordable = canAfford;
            } else if (IsSelected && !canAfford)
            {
                IsSelected = false;
                GetComponent<Image>().color = _unavailableColor;
                IsAffordable = false;
            }
        }

        public void Initialize(PlanetDataSO planetData, PlanetListInitializer parentInitializer)
        {
            PlanetData = planetData;
            _parentInitializer = parentInitializer;
            _planetSprite.sprite = planetData.sprite;
            _planetName.text = planetData.constructibleName;
            
            _storageSystem = WorldManager.Instance.Player.StorageSystem;
            var production = "";
            for (int i = 0; i < planetData.producibleResources.Count; i++)
            {
                production += $"{planetData.resourcePerCycle[i]} {planetData.producibleResources[i].DisplayName} / cycle";
                if (i < planetData.producibleResources.Count - 1)
                    production += ", \n";
            }
            
            UpdatePrefabAvailability();
            _planetProductionResource.text = production;
            
            _costPerResource = planetData.buildCostAmounts;
            _resourceCostInitializer.Initialize(planetData, _costPerResource);
        }
        
        public void UpdateCosts(float discountPercentage)
        {
            totalDiscount += discountPercentage;
            int[] newCosts = new int[_costPerResource.Length];
            for (int i = 0; i < _costPerResource.Length; i++)
            {
                float discountedCost = _costPerResource[i] * (1 - totalDiscount / 100f);
                newCosts[i] = Mathf.CeilToInt(discountedCost);
            }
            _resourceCostInitializer.UpdateCosts(newCosts);
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
                GetComponent<Image>().color = _selectedColor;
            }
            else
            {
                GetComponent<Image>().color = _availableColor;
            }
        }
    }
}