using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.UI.Windows;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class StarsInitializer : MonoBehaviour, ISaveable
    {
        [SerializeField] private List<STARSDataSO> _starsDataSOs; 
        [SerializeField] private StarsListPrefab _starsListPrefab;
        
        [Header("Botón de Construcción")]
        [SerializeField] private Button buildStarsButton;
        
        private StarsListPrefab _currentStarsItem;
        
        private StorageSystem _storageSystem;
        private SolarSystem _solarSystem;
        private Dictionary<string, StarsListPrefab> _createdStars = new Dictionary<string, StarsListPrefab>();
        private bool _isStarsBuilt = false;

        private void Start()
        {
            _storageSystem = WorldManager.Instance.Player.StorageSystem;
            _solarSystem = ServiceLocator.GetService<SolarSystem>();

            foreach (var starsData in _starsDataSOs)
            {
                AddNewStars(starsData);
            }
            
            if (buildStarsButton != null)
            {
                buildStarsButton.gameObject.SetActive(false);
                buildStarsButton.onClick.AddListener(OnBuildStarsClicked);
            }
        }

        private void AddNewStars(STARSDataSO obj)
        {
            if (obj == null) return;
            
            if (_createdStars.ContainsKey(obj.constructibleName))
            {
                return;
            }
            
            StarsListPrefab starsItem = Instantiate(_starsListPrefab, transform);
            starsItem.Initialize(obj, this);
            
            _createdStars.Add(obj.constructibleName, starsItem);
        }

        public void SetCurrentStarsItem(StarsListPrefab starsItem)
        {
            if (_currentStarsItem != null)
            {
                _currentStarsItem.IsSelected = false;
                _currentStarsItem.UpdateVisualState();
            }

            _currentStarsItem = starsItem;
            _currentStarsItem.IsSelected = true;
            _currentStarsItem.UpdateVisualState();
            
            if (buildStarsButton != null)
            {
                buildStarsButton.gameObject.SetActive(true);
                UpdateBuildButtonState();
            }
        }
        
        public STARSDataSO GetCurrentStarsData()
        {
            if (_currentStarsItem == null)
                return null;

            return _currentStarsItem.StarsData;
        }

        private bool CheckAffordability(STARSDataSO starsData)
        {
            if (starsData == null) return false;
            for (int i = 0; i < starsData.buildCostAmounts.Length; i++)
            {
                if (!_storageSystem.HasResource(starsData.buildCostResources[i].Type, starsData.buildCostAmounts[i]))
                {
                    return false;
                }
            }
            return true;
        }
        
        public bool ConsumeResourcesForStars(STARSDataSO starsData)
        {
            if (starsData == null) return false;
            if (!CheckAffordability(starsData))
            {
                return false;
            }
            
            for (int i = 0; i < starsData.buildCostAmounts.Length; i++)
            {
                if (!_storageSystem.HasResource(starsData.buildCostResources[i].Type, starsData.buildCostAmounts[i]))
                {
                    return false;
                }
            }
            for (int i = 0; i < starsData.buildCostAmounts.Length; i++)
            {
                _storageSystem.ConsumeResource(
                    starsData.buildCostResources[i].Type, 
                    starsData.buildCostAmounts[i]
                );
            }
            return true;
        }
        
        public void ClearSelection()
        {
            if (_currentStarsItem != null)
            {
                _currentStarsItem.IsSelected = false;
                _currentStarsItem.UpdateVisualState();
                _currentStarsItem = null;
            }
            
            if (buildStarsButton != null)
            {
                buildStarsButton.gameObject.SetActive(false);
            }
        }
        
        public void UpdateBuildButtonState()
        {
            if (_currentStarsItem == null || buildStarsButton == null) return;

            bool canAfford = CheckAffordability(_currentStarsItem.StarsData);
            buildStarsButton.interactable = canAfford;
        }
        
        private void OnBuildStarsClicked()
        {
            if (_currentStarsItem == null) return;

            var starsData = _currentStarsItem.StarsData;
            
            if (ConsumeResourcesForStars(starsData))
            {
                //_solarSystem.BuildSpecialPlanet(starsData);
                _isStarsBuilt = true;
                Destroy(_currentStarsItem.gameObject);
                _createdStars.Remove(starsData.constructibleName);
                _currentStarsItem = null;
                
                ClearSelection();
                
                var panelManager = GetComponentInParent<StarsPanelUnlocker>();
                if (panelManager != null)
                {
                    panelManager.gameObject.SetActive(false);
                }

                UIManager.Instance.ShowScreen<InGameScreen>();
            }
        }

        public string GetSaveId()
        {
            return "StarsInitializer";
        }

        public JToken CaptureState()
        {
            JObject state = new JObject
            {
                ["isStarsBuilt"] = _isStarsBuilt
            };
            return state;
        }

        public void RestoreState(JToken state)
        {
            bool isStarsBuilt = state.Value<bool>("isStarsBuilt");
            _isStarsBuilt = isStarsBuilt;

            if (_isStarsBuilt)
            {
                var starsData = _starsDataSOs[0];
                
                _solarSystem = ServiceLocator.GetService<SolarSystem>();
                _solarSystem.BuildSpecialPlanet(starsData);
                _currentStarsItem = null;
                
                ClearSelection();
                
                var panelManager = GetComponentInParent<StarsPanelUnlocker>();
                if (panelManager != null)
                {
                    panelManager.gameObject.SetActive(false);
                }

            }
        }
    }
}