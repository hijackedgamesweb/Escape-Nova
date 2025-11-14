using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Construction
{
    public class ConstructionUI : BaseUIScreen
    {
        //VARIABLES
        [SerializeField] private GameObject placingUI;
    
        [SerializeField] private GameObject PlanetsTab;
        [SerializeField] private GameObject SatelitesTab;
        private int currentTab = 0; //0: Planets / 1: Satelites
    
        [SerializeField] private Button buildBtn;
        [SerializeField] public Button exitBtn;
        [SerializeField] private GameObject errorMsg;
    
        [SerializeField] private GameObject planetsLayoutGroup;
        [SerializeField] private GameObject satelitesLayoutGroup;
    
        [SerializeField] private GameObject entitieConstructionButtonPrefab;
        
        [Header("Datos de Construcción")]
        [SerializeField] private List<PlanetDataSO> availablePlanets;
        [SerializeField] private List<SateliteDataSO> availableSatelites;
        
        private EntityConstructionButton _selectedButton;
        private SolarSystem _solarSystem;
        private PlacingUI _placingUIScript;
        
        private StorageSystem _storageSystem;
    
        //METODOS
        private void Awake()
        {
            
        }

        private void Start()
        {
            _solarSystem = ServiceLocator.GetService<SolarSystem>();
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            
            if (placingUI != null)
            {
                _placingUIScript = placingUI.GetComponent<PlacingUI>();
            }

            foreach (var planetData in availablePlanets)
            {
                GameObject plnt = Instantiate(entitieConstructionButtonPrefab, planetsLayoutGroup.transform, false);
                plnt.GetComponent<EntityConstructionButton>().Initialize(planetData);
            }
            
            foreach (var sateliteData in availableSatelites)
            {
                GameObject stlt = Instantiate(entitieConstructionButtonPrefab, satelitesLayoutGroup.transform, false);
                stlt.GetComponent<EntityConstructionButton>().Initialize(sateliteData);
            }
            
            buildBtn.interactable = false;
        }
        
        public void TabPressed(int idx)
        {
            switch (idx)
            {
                case 0:
                    planetsLayoutGroup.SetActive(true);
                    satelitesLayoutGroup.SetActive(false);
                    currentTab = 0; //Current Tab -> Planets
                    break;
                case 1:
                    planetsLayoutGroup.SetActive(false);
                    satelitesLayoutGroup.SetActive(true);
                    currentTab = 1; //Current Tab -> Satelites
                    break;
            }
        }
    
    
        public void EntityPressed(EntityConstructionButton pressedButton)
        {
            buildBtn.interactable = true;
            _selectedButton = pressedButton;
        }
        public void BuildButtonPressed()
        {
            if (_selectedButton == null) return;
            if (_placingUIScript == null)
            {
                return;
            }

            if (CheckForReources()) 
            {
                ConstructibleDataSO dataToBuild = _selectedButton.entityData;
                for (int i = 0; i < dataToBuild.buildCostResources.Length; i++)
                {
                    ResourceData resource = dataToBuild.buildCostResources[i];
                    int amount = dataToBuild.buildCostAmounts[i];
                    
                    _storageSystem.ConsumeResource(resource.Type, amount); 
                }
                _placingUIScript.entityToBuild = dataToBuild;
                placingUI.SetActive(true);
                gameObject.SetActive(false); 
                
                if(errorMsg.activeSelf) { errorMsg.SetActive(false); }
            }
            else 
            {
                errorMsg.SetActive(true); 
            }
        }
    
    
        private bool CheckForReources()
        {
            if (_storageSystem == null)
            {
                return false;
            }
            if (_selectedButton == null)
            {
                return false;
            }
            ConstructibleDataSO dataToBuild = _selectedButton.entityData;
            if (dataToBuild.buildCostResources.Length != dataToBuild.buildCostAmounts.Length)
            {
                return false;
            }
            for (int i = 0; i < dataToBuild.buildCostResources.Length; i++)
            {
                ResourceData resource = dataToBuild.buildCostResources[i];
                int requiredAmount = dataToBuild.buildCostAmounts[i];
                if (!_storageSystem.HasResource(resource.Type, requiredAmount))
                {
                    return false; 
                }
            }
            
            return true;
        }
    }
}