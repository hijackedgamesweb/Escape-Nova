using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.UI.Crafting
{
    public class CraftingPanelUI : MonoBehaviour
    {
        private CraftingSystem _craftingSystem;
        private StorageSystem _storageSystem;

        [Header("Referencias UI")]
        [SerializeField] private GameObject recipeButtonPrefab;
        [SerializeField] private GameObject ingredientSlotPrefab;
        [SerializeField] private Transform recipeListContainer;
        [SerializeField] private Transform ingredientsContainer;
        
        [Header("Detalles Receta")]
        [SerializeField] private Image detailIcon;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private TextMeshProUGUI detailTimeText;
        [SerializeField] private Button craftButton;
        [SerializeField] private TMP_InputField craftAmountInput;
        
        [SerializeField] private Slider craftingProgressBar;
        [SerializeField] private GameObject placeholderTextObject;

        private List<CraftingRecipeUIItem> _recipeButtons = new List<CraftingRecipeUIItem>();
        private List<IngredientSlotUI> _activeIngredientSlots = new List<IngredientSlotUI>();
        
        private CraftingRecipe _selectedRecipe;

        private bool _shouldUpdateButtons;
        private bool _shouldRefreshRecipeList;
        private bool _shouldUpdateDetails;
        
        private float _lastButtonUpdateTime;
        private const float BUTTON_UPDATE_INTERVAL = 0.2f;
        
        void Start()
        {
            _craftingSystem = ServiceLocator.GetService<CraftingSystem>();
            if (WorldManager.Instance != null && WorldManager.Instance.Player != null)
            {
                _storageSystem = WorldManager.Instance.Player.StorageSystem;
            }
            
            if (_craftingSystem == null || _storageSystem == null) return;

            _craftingSystem.OnRecipeUnlocked += OnRecipeUnlocked;
            _craftingSystem.OnItemCrafted += OnItemCrafted;
            _storageSystem.OnStorageUpdated += OnStorageUpdated;
            _craftingSystem.OnCraftingStarted += OnCraftingStarted;
            _craftingSystem.OnCraftingCompleted += OnCraftingCompleted;
            
            craftButton.onClick.AddListener(OnCraftButtonClicked);
            
            if (craftAmountInput != null)
            {
                craftAmountInput.onValueChanged.AddListener(OnAmountInputChanged);
                craftAmountInput.onEndEdit.AddListener(OnAmountInputChanged);
            }
            
            if (craftingProgressBar != null) craftingProgressBar.gameObject.SetActive(false);
            if (placeholderTextObject != null) placeholderTextObject.SetActive(false);

            RefreshRecipeList();
            UpdateCraftButtonState();
        }

        private void OnDestroy()
        {
            if (_craftingSystem != null)
            {
                _craftingSystem.OnRecipeUnlocked -= OnRecipeUnlocked;
                _craftingSystem.OnItemCrafted -= OnItemCrafted;
                _craftingSystem.OnCraftingStarted -= OnCraftingStarted;
                _craftingSystem.OnCraftingCompleted -= OnCraftingCompleted;
            }
            if (_storageSystem != null)
            {
                _storageSystem.OnStorageUpdated -= OnStorageUpdated;
            }
            
            if (craftAmountInput != null)
            {
                craftAmountInput.onValueChanged.RemoveListener(OnAmountInputChanged);
                craftAmountInput.onEndEdit.RemoveListener(OnAmountInputChanged);
            }
        }

        private void LateUpdate()
        {
            float realTime = Time.unscaledTime;

            if (_shouldRefreshRecipeList)
            {
                RefreshRecipeList();
                _shouldRefreshRecipeList = false;
            }

            if (_shouldUpdateDetails && _selectedRecipe != null)
            {
                UpdateRecipeTimeText(_selectedRecipe);
                RefreshIngredientValuesOnly(_selectedRecipe);
                UpdateCraftButtonState();
                _shouldUpdateDetails = false;
            }

            if (_shouldUpdateButtons)
            {
                if (realTime - _lastButtonUpdateTime > BUTTON_UPDATE_INTERVAL || !_craftingSystem.IsAnyCraftingInProgress())
                {
                    UpdateAllButtonCraftableStatus();
                    UpdateCraftButtonState();
                    
                    if (!_craftingSystem.IsAnyCraftingInProgress() && craftingProgressBar != null)
                    {
                        craftingProgressBar.gameObject.SetActive(false);
                    }

                    if (_selectedRecipe != null && !_shouldUpdateDetails)
                    {
                        RefreshIngredientValuesOnly(_selectedRecipe);
                    }
                    
                    _lastButtonUpdateTime = realTime;
                    _shouldUpdateButtons = false;
                }
            }

            if (craftingProgressBar != null && _selectedRecipe != null)
            {
                bool isRecipeActive = _craftingSystem.GetCurrentCraftingRecipeId() == _selectedRecipe.recipeId;
                
                if (isRecipeActive)
                {
                    if (!craftingProgressBar.gameObject.activeSelf) 
                        craftingProgressBar.gameObject.SetActive(true);
                    
                    craftingProgressBar.value = _craftingSystem.GetCurrentCraftingProgress();
                }
            }
        }
        
        private void OnAmountInputChanged(string value)
        {
            _shouldUpdateDetails = true;
        }

        private void OnRecipeUnlocked(string recipeId) => _shouldRefreshRecipeList = true;
        private void OnItemCrafted(string recipeId, int amount) => _shouldUpdateButtons = true;
        private void OnStorageUpdated() => _shouldUpdateButtons = true;
        private void OnCraftingStarted(string recipeId)
        {
            _shouldUpdateButtons = true;
            if (craftingProgressBar != null) craftingProgressBar.value = 0;
        }
        private void OnCraftingCompleted(string recipeId)
        {
            _lastButtonUpdateTime = 0; 
            _shouldUpdateButtons = true;
        }

        // --- Lógica de UI ---

        private void RefreshRecipeList()
        {
            foreach (Transform child in recipeListContainer) Destroy(child.gameObject);
            _recipeButtons.Clear();

            if (_craftingSystem == null) return;
            var unlockedRecipes = _craftingSystem.GetAllUnlockedRecipes();
            
            if (placeholderTextObject != null) placeholderTextObject.SetActive(unlockedRecipes.Count == 0);
            
            foreach (var recipe in unlockedRecipes)
            {
                var buttonGO = Instantiate(recipeButtonPrefab, recipeListContainer);
                var buttonUI = buttonGO.GetComponent<CraftingRecipeUIItem>();
                buttonUI.Initialize(recipe, this, _craftingSystem);
                _recipeButtons.Add(buttonUI);
            }

            if (_selectedRecipe != null) SelectRecipe(_selectedRecipe);
        }
        
        private void UpdateAllButtonCraftableStatus()
        {
            foreach(var button in _recipeButtons) button.UpdateCraftableStatus();
        }

        public void SelectRecipe(CraftingRecipe recipe)
        {
            _selectedRecipe = recipe;
            
            foreach (var button in _recipeButtons)
            {
                button.SetSelected(button.RecipeId == recipe.recipeId);
            }
            
            if (placeholderTextObject != null) placeholderTextObject.SetActive(false);

            var outputItemData = _craftingSystem.GetItemData(recipe.output.itemName);
            if (outputItemData != null)
            {
                detailIcon.sprite = outputItemData.icon;
                detailName.text = outputItemData.displayName;
                detailDescription.text = outputItemData.description;
            }
            
            // Actualizamos tiempo y slots
            UpdateRecipeTimeText(recipe);
            RebuildIngredientSlots(recipe);
            
            UpdateCraftButtonState();
            
            if (craftingProgressBar != null)
            {
                if (_craftingSystem.IsAnyCraftingInProgress() && _craftingSystem.GetCurrentCraftingRecipeId() == recipe.recipeId)
                {
                    craftingProgressBar.gameObject.SetActive(true);
                    craftingProgressBar.value = _craftingSystem.GetCurrentCraftingProgress();
                }
                else
                {
                    craftingProgressBar.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateRecipeTimeText(CraftingRecipe recipe)
        {
            if (detailTimeText != null)
            {
                const float secondsPerCycle = 5.0f;
                int amount = GetCraftAmount();
                
                // Calculamos tiempo total basado en la cantidad
                float totalSeconds = recipe.craftingTimeInSeconds * amount;
                float cyclesAsFloat = totalSeconds / secondsPerCycle;
                int displayCycles = Mathf.CeilToInt(cyclesAsFloat);
                
                detailTimeText.text = $"Time: {displayCycles} cycles";
            }
        }

        private void RebuildIngredientSlots(CraftingRecipe recipe)
        {
            foreach (Transform child in ingredientsContainer) Destroy(child.gameObject);
            _activeIngredientSlots.Clear();

            foreach (var ingredient in recipe.ingredients)
            {
                var slotGO = Instantiate(ingredientSlotPrefab, ingredientsContainer);
                var slotUI = slotGO.GetComponent<IngredientSlotUI>();
                _activeIngredientSlots.Add(slotUI);
                
                SetSlotData(slotUI, ingredient);
            }
        }

        private void RefreshIngredientValuesOnly(CraftingRecipe recipe)
        {
            if (_activeIngredientSlots.Count != recipe.ingredients.Count)
            {
                RebuildIngredientSlots(recipe);
                return;
            }

            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                if (_activeIngredientSlots[i] != null)
                {
                    SetSlotData(_activeIngredientSlots[i], recipe.ingredients[i]);
                }
            }
        }

        private void SetSlotData(IngredientSlotUI slotUI, CraftingIngredient ingredient)
        {
            Sprite ingredientIcon = null;
            string ingredientName = "";
            int amountOwned = 0;

            if (ingredient.useInventoryItem)
            {
                var itemData = _craftingSystem.GetItemData(ingredient.itemName);
                if (itemData != null) { ingredientIcon = itemData.icon; ingredientName = itemData.displayName; }
                amountOwned = _storageSystem.GetInventoryItemQuantity(ingredient.itemName);
            }
            else
            {
                var resourceData = _storageSystem.GetResourceData(ingredient.resourceType);
                if (resourceData != null) { ingredientIcon = resourceData.Icon; ingredientName = resourceData.DisplayName; }
                amountOwned = _storageSystem.GetResourceAmount(ingredient.resourceType);
            }
            
            int currentMultiplier = GetCraftAmount();
            int totalRequired = ingredient.amount * currentMultiplier;
            
            slotUI.SetData(ingredientIcon, ingredientName, amountOwned, totalRequired);
        }
        
        private int GetCraftAmount()
        {
            if (craftAmountInput == null) return 1;
            if (int.TryParse(craftAmountInput.text, out int amount))
            {
                if (amount <= 0) return 1;
                return amount;
            }
            return 1;
        }

        private void UpdateCraftButtonState()
        {
            if (_selectedRecipe == null)
            {
                craftButton.gameObject.SetActive(false);
                return;
            }
            craftButton.gameObject.SetActive(true);
            
            if (_craftingSystem.IsAnyCraftingInProgress())
            {
                craftButton.interactable = false;
                return;
            }

            int amountToCraft = GetCraftAmount(); 
            bool canCraft = _craftingSystem.CanCraft(_selectedRecipe.recipeId, amountToCraft);
            craftButton.interactable = canCraft;
        }

        private void OnCraftButtonClicked()
        {
            if (_selectedRecipe == null) return;
            int amountToCraft = GetCraftAmount();
            
            bool didCraftStart = _craftingSystem.Craft(_selectedRecipe.recipeId, amountToCraft);

            if (didCraftStart)
            {
                UpdateCraftButtonState(); 
            }
        }
    }
}