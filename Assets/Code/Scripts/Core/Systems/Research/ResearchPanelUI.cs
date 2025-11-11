using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Research
{
    public class ResearchPanelUI : BaseUIScreen 
    {
        [Header("Pestañas (Tabs)")]
        [SerializeField] private Button planetasTabButton;
        [SerializeField] private Button objetosTabButton;
        [SerializeField] private Button satelitesTabButton;
        [SerializeField] private Button especialTabButton;

        [Header("Estilo de Pestañas")]
        [SerializeField] private Color tabNormalColor = Color.gray;
        [SerializeField] private Color tabSelectedColor = Color.white;

        [Header("Lista de Items (Izquierda)")]
        [SerializeField] private Transform researchItemsContainer;
        [SerializeField] private GameObject researchItemPrefab;

        [Header("Panel de Detalles (Derecha)")]
        [SerializeField] private GameObject detailsPanel;
        [SerializeField] private Image detailIcon;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private TextMeshProUGUI detailTimeText;
        [SerializeField] private Transform ingredientsContainer;
        [SerializeField] private GameObject ingredientSlotPrefab;
        [SerializeField] private Button startResearchButton;
        
        [Header("Displays (Global)")]
        [SerializeField] private Slider researchProgressSlider;
        [SerializeField] private TextMeshProUGUI resourcesText;

        private ResearchSystem _researchSystem;
        private StorageSystem _storageSystem;
        
        private Dictionary<string, ResearchUIItem> _researchUIItems = new Dictionary<string, ResearchUIItem>();
        private bool isInitialized = false;
        private ResearchCategory _currentCategory = ResearchCategory.Planetas;
        
        private ResearchNode _selectedNode;
        private ResearchUIItem _selectedItemButton;
        private Button _currentSelectedTab;

        public override void Show(object parameter = null)
        {
            base.Show(parameter);
            if (!isInitialized)
            {
                Initialize();
            }
            SelectCategory(_currentCategory);
            UpdateAllUI();
        }
        
        public override void Hide()
        {
            base.Hide();
            if (detailsPanel != null)
                detailsPanel.SetActive(false);
            _selectedNode = null;
        }

        private void Initialize()
        {
            try
            {
                _researchSystem = ServiceLocator.GetService<ResearchSystem>();
                _storageSystem = ServiceLocator.GetService<StorageSystem>();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ResearchPanelUI no pudo inicializarse: {e.Message}");
                return;
            }

            _researchSystem.OnResearchStarted += OnResearchUpdate;
            _researchSystem.OnResearchProgress += OnResearchUpdate;
            _researchSystem.OnResearchCompleted += OnResearchUpdate;
            _researchSystem.OnResearchUnlocked += OnResearchUnlocked;
            _storageSystem.OnStorageUpdated += OnStorageUpdated;

            planetasTabButton.onClick.AddListener(() => SelectCategory(ResearchCategory.Planetas));
            objetosTabButton.onClick.AddListener(() => SelectCategory(ResearchCategory.Objetos));
            satelitesTabButton.onClick.AddListener(() => SelectCategory(ResearchCategory.Satelites));
            especialTabButton.onClick.AddListener(() => SelectCategory(ResearchCategory.Especial));
            
            startResearchButton.onClick.AddListener(OnStartResearchButtonClicked);

            if (planetasTabButton != null)
            {
                _currentSelectedTab = planetasTabButton; 
            }
            
            BuildFullResearchList();
            isInitialized = true;
        }

        private void BuildFullResearchList()
        {
            foreach (Transform child in researchItemsContainer) Destroy(child.gameObject);
            _researchUIItems.Clear();

            var allResearch = _researchSystem.GetAllResearchStatus();

            foreach (var researchStatus in allResearch)
            {
                ResearchNode node = _researchSystem.GetResearch(researchStatus.Key);
                
                if (node != null) 
                {
                    GameObject uiItemGO = Instantiate(researchItemPrefab, researchItemsContainer);
                    var researchItem = uiItemGO.GetComponent<ResearchUIItem>();
                    
                    researchItem.Initialize(node, _researchSystem, this); 
                    _researchUIItems[node.researchId] = researchItem;
                    uiItemGO.SetActive(false);
                }
            }
        }
        
        public void SelectCategory(ResearchCategory category)
        {
            _currentCategory = category;
            
            detailsPanel.SetActive(false);
            _selectedNode = null;
            
            if (_selectedItemButton != null)
            {
                _selectedItemButton.SetSelected(false);
                _selectedItemButton = null;
            }
            
            if (planetasTabButton != null) planetasTabButton.GetComponent<Image>().color = tabNormalColor;
            if (objetosTabButton != null) objetosTabButton.GetComponent<Image>().color = tabNormalColor;
            if (satelitesTabButton != null) satelitesTabButton.GetComponent<Image>().color = tabNormalColor;
            if (especialTabButton != null) especialTabButton.GetComponent<Image>().color = tabNormalColor;

            switch (category)
            {
                case ResearchCategory.Planetas: _currentSelectedTab = planetasTabButton; break;
                case ResearchCategory.Objetos: _currentSelectedTab = objetosTabButton; break;
                case ResearchCategory.Satelites: _currentSelectedTab = satelitesTabButton; break;
                case ResearchCategory.Especial: _currentSelectedTab = especialTabButton; break;
            }

            if (_currentSelectedTab != null)
            {
                _currentSelectedTab.GetComponent<Image>().color = tabSelectedColor;
            }

            ResearchUIItem firstItemInThisCategory = null;

            foreach (var pair in _researchUIItems)
            {
                var item = pair.Value;
                var node = item.GetNodeData();

                if (node.category == _currentCategory)
                {
                    item.gameObject.SetActive(true);
                    if (firstItemInThisCategory == null)
                    {
                        firstItemInThisCategory = item;
                    }
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
            
            if (firstItemInThisCategory != null)
            {
                OnItemClicked(firstItemInThisCategory);
            }
        }
        
        public void OnItemClicked(ResearchUIItem item)
        {
            if (item == null) return;
            
            if (_selectedItemButton != null)
            {
                _selectedItemButton.SetSelected(false);
            }
            
            _selectedItemButton = item;
            _selectedItemButton.SetSelected(true);
            
            ShowDetails(item.GetNodeData());
        }

        private void ShowDetails(ResearchNode node)
        {
            _selectedNode = node;
            detailsPanel.SetActive(true);

            detailIcon.sprite = node.icon; 
            detailName.text = node.displayName;
            detailDescription.text = node.description;
            detailTimeText.text = $"Tiempo: {node.researchTimeInSeconds}s";

            foreach (Transform child in ingredientsContainer) Destroy(child.gameObject);
            foreach (var cost in node.resourceCosts)
            {
                var slotGO = Instantiate(ingredientSlotPrefab, ingredientsContainer);
                var slotUI = slotGO.GetComponent<IngredientSlotUI>();
                
                Sprite costIcon = null;
                string costName = "";
                int amountOwned = 0;

                if (cost.useInventoryItem)
                {
                    var itemData = _storageSystem.GetItemData(cost.itemName);
                    if (itemData != null) { costIcon = itemData.icon; costName = itemData.displayName; }
                    amountOwned = _storageSystem.GetInventoryItemQuantity(cost.itemName);
                }
                else
                {
                    var resourceData = _storageSystem.GetResourceData(cost.resourceType);
                    if (resourceData != null) { costIcon = resourceData.Icon; costName = resourceData.DisplayName; }
                    amountOwned = _storageSystem.GetResourceAmount(cost.resourceType);
                }
                
                slotUI.SetData(costIcon, costName, amountOwned, cost.amount);
            }
            
            UpdateStartResearchButtonState();
        }

        private void UpdateStartResearchButtonState()
        {
            if (_selectedNode == null) return;
            
            startResearchButton.interactable = _researchSystem.CanStartResearch(_selectedNode.researchId);

            var status = _researchSystem.GetResearchStatus(_selectedNode.researchId);
            var buttonText = startResearchButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                switch (status)
                {
                    case ResearchStatus.Available:
                        buttonText.text = "Investigar";
                        break;
                    case ResearchStatus.Locked:
                        buttonText.text = "Bloqueado";
                        break;
                    case ResearchStatus.InProgress:
                        buttonText.text = "En Progreso";
                        break;
                    case ResearchStatus.Completed:
                        buttonText.text = "Completado";
                        break;
                }
                if (_researchSystem.IsAnyResearchInProgress() && status == ResearchStatus.Available)
                {
                    buttonText.text = "Investigación Ocupada";
                }
            }
        }
        
        private void OnStartResearchButtonClicked()
        {
            if (_selectedNode == null) return;
            
            if (_researchSystem.CanStartResearch(_selectedNode.researchId))
            {
                _researchSystem.StartResearch(_selectedNode.researchId);
            }
        }
        
        private void OnResearchUpdate(string researchId)
        {
            UpdateAllUI();
        }
        private void OnResearchUpdate(string researchId, float progress)
        {
            if (researchId == _researchSystem.GetCurrentResearchId() && researchProgressSlider != null)
            {
                researchProgressSlider.gameObject.SetActive(true);
                researchProgressSlider.value = progress;
            }

            if (_researchUIItems.TryGetValue(researchId, out ResearchUIItem item))
            {
                item.UpdateButtonState();
            }
        }
        
        private void OnResearchUnlocked(string researchId)
        {
            var node = _researchSystem.GetResearch(researchId);
            if (node != null && _researchUIItems.ContainsKey(researchId))
            {
                 _researchUIItems[researchId].UpdateButtonState();
            }
        }

        private void OnStorageUpdated()
        {
            UpdateAllUI();
        }
        
        private void UpdateAllUI()
        {
            if (!isInitialized) return;
            
            if (_selectedNode != null)
            {
                ShowDetails(_selectedNode);
            }
            
            foreach (var item in _researchUIItems.Values)
            {
                item.UpdateButtonState();
            }
            
            UpdateResourcesUI();
            UpdateCurrentResearchProgressDisplay();
        }
        
        private void UpdateResourcesUI()
        {
            if (resourcesText == null || _storageSystem == null) return;
            var resources = _storageSystem.GetAllResources();
            string resourcesInfo = "Recursos: ";
            foreach (var resource in resources)
            {
                resourcesInfo += $"{resource.Key}: {resource.Value} ";
            }
            resourcesText.text = resourcesInfo;
        }

        private void UpdateCurrentResearchProgressDisplay()
        {
            if (researchProgressSlider == null || _researchSystem == null) return;

            if (_researchSystem.IsAnyResearchInProgress())
            {
                string id = _researchSystem.GetCurrentResearchId();
                float progress = _researchSystem.GetResearchProgress(id);
                researchProgressSlider.gameObject.SetActive(true);
                researchProgressSlider.value = progress;
            }
            else
            {
                researchProgressSlider.gameObject.SetActive(false);
            }
        }
        
        private void OnDestroy()
        {
            if (_researchSystem != null)
            {
                _researchSystem.OnResearchStarted -= OnResearchUpdate;
                _researchSystem.OnResearchProgress -= OnResearchUpdate;
                _researchSystem.OnResearchCompleted -= OnResearchUpdate;
                _researchSystem.OnResearchUnlocked -= OnResearchUnlocked;
            }
            if (_storageSystem != null)
            {
                _storageSystem.OnStorageUpdated -= OnStorageUpdated;
            }
        }
    }
}