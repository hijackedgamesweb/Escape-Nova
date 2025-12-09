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
    public class SateliteListPrefab : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _planetSprite;
        [SerializeField] private TMP_Text _planetName;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private ResourceCostInitializer _resourceCostInitializer;
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _availableColor = Color.red;
        [SerializeField] private Color _unavailableColor = Color.gray;
        
        private int[] _costPerResource;
        private float totalDiscount;
        
        SateliteListInitializer _parentInitializer;
        public bool IsSelected { get; set; }
        public SateliteDataSO SateliteData { get; set; }
        private StorageSystem _storageSystem;
        
        public bool IsAffordable { get; set; }
        private IGameTime _gameTime;
        
        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnCycleCompleted += UpdatePrefabAvailability;
        }
        
        public void Initialize(SateliteDataSO sateliteData, SateliteListInitializer parentInitializer)
        {
            
            _storageSystem = WorldManager.Instance.Player.StorageSystem;
            SateliteData = sateliteData;
            _parentInitializer = parentInitializer;
            _planetSprite.sprite = sateliteData.sprite;
            _planetName.text = sateliteData.constructibleName;
            _descriptionText.text = sateliteData.desc;
            _costPerResource = sateliteData.buildCostAmounts;
            _resourceCostInitializer.Initialize(sateliteData, _costPerResource);
            
            UpdatePrefabAvailability();
        }
        
        private void UpdatePrefabAvailability(int obj = 0)
        {
            bool canAfford = true;
            for (int i = 0; i < SateliteData.buildCostAmounts.Length; i++)
            {
                ResourceType resource = SateliteData.buildCostResources[i].Type; 
                var cost = SateliteData.buildCostAmounts[i];
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
                GetComponent<SpriteRenderer>().color = canAfford ? _availableColor : _unavailableColor;
                IsAffordable = canAfford;
            } else if (IsSelected && !canAfford)
            {
                IsSelected = false;
                GetComponent<SpriteRenderer>().color = _unavailableColor;
                IsAffordable = false;
            }
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
                _parentInitializer.SetCurrentSateliteItem(this);
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