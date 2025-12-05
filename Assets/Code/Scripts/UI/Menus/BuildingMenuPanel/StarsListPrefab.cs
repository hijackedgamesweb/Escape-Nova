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
    public class StarsListPrefab : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _starsIcon;
        [SerializeField] private TMP_Text _starsName;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private ResourceCostInitializer _resourceCostInitializer;

        private int[] _costPerResource;
        private float totalDiscount;

        private StarsInitializer _parentInitializer;
        public bool IsSelected { get; set; }
        public STARSDataSO StarsData { get; private set; }
        private StorageSystem _storageSystem;

        public bool IsAffordable { get; set; }
        private IGameTime _gameTime;

        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnCycleCompleted += UpdatePrefabAvailability;
        }
        
        private void OnDestroy()
        {
            if (_gameTime != null)
            {
                _gameTime.OnCycleCompleted -= UpdatePrefabAvailability;
            }
        }

        public void Initialize(STARSDataSO starsData, StarsInitializer parentInitializer)
        {
            _storageSystem = WorldManager.Instance.Player.StorageSystem;
            StarsData = starsData;
            _parentInitializer = parentInitializer;
            _starsIcon.sprite = starsData.sprite; 
            _starsName.text = starsData.constructibleName;
            _descriptionText.text = starsData.desc;
            _costPerResource = starsData.buildCostAmounts;
            _resourceCostInitializer.Initialize(starsData, _costPerResource);

            UpdatePrefabAvailability();
        }

        private void UpdatePrefabAvailability(int obj = 0)
        {
            bool canAfford = true;
            for (int i = 0; i < StarsData.buildCostAmounts.Length; i++)
            {
                ResourceType resource = StarsData.buildCostResources[i].Type;
                var cost = StarsData.buildCostAmounts[i];
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
            
            IsAffordable = canAfford;

            if (!IsSelected)
            {
                _starsIcon.color = canAfford ? Color.white : Color.red;
            }
            else if (IsSelected && !canAfford)
            {
                IsSelected = false;
                _starsIcon.color = Color.red;
                _parentInitializer.ClearSelection();
            }
            
            if (IsSelected)
            {
                _parentInitializer.UpdateBuildButtonState();
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
            UpdatePrefabAvailability();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsAffordable) return;
            
            if (IsSelected)
            {
                _parentInitializer.ClearSelection();
            }
            else
            {
                _parentInitializer.SetCurrentStarsItem(this);
            }
        }

        public void UpdateVisualState()
        {
            if (IsSelected)
            {
                _starsIcon.color = Color.yellow;
            }
            else
            {
                _starsIcon.color = IsAffordable ? Color.white : Color.red;
            }
        }
    }
}