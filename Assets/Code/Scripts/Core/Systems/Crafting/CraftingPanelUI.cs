using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Crafting;
using Newtonsoft.Json.Linq;

public class CraftingPanelUI : MonoBehaviour, ISaveable
{
    private CraftingSystem _craftingSystem;
    private StorageSystem _storageSystem;

    [Header("Prefabs")]
    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private GameObject ingredientSlotPrefab;

    [Header("Containers")]
    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private Transform ingredientsContainer;
    
    [Header("Recipe Details")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    
    [Tooltip("Texto para mostrar el tiempo total estimado (ej: 15s)")]
    [SerializeField] private TextMeshProUGUI craftingTimeText; 

    [Header("Controls")]
    [SerializeField] private Button craftButton;
    [SerializeField] private TMP_InputField craftAmountInput;
    [SerializeField] private Slider craftingProgressBar;
    [SerializeField] private GameObject placeholderTextObject;

    private List<CraftingRecipeUIItem> _recipeButtons = new List<CraftingRecipeUIItem>();
    private List<IngredientSlotUI> _activeIngredientSlots = new List<IngredientSlotUI>();
    
    private CraftingRecipe _selectedRecipe;
    private bool _isCrafting = false;
    void Start()
    {
        _craftingSystem = ServiceLocator.GetService<CraftingSystem>();
        
        if (WorldManager.Instance != null && WorldManager.Instance.Player != null)
        {
            _storageSystem = WorldManager.Instance.Player.StorageSystem;
        }
        
        if (_craftingSystem == null || _storageSystem == null)
        {
            Debug.LogError("[CraftingPanelUI] Sistemas no encontrados.");
            return;
        }

        SubscribeEvents();
        InitializeUI();
        RefreshRecipeList();
        UpdateCraftButtonState();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    void Update()
    {
        if (_craftingSystem == null) return;

        if (_isCrafting)
        {
            if (craftingProgressBar != null)
            {
                craftingProgressBar.value = _craftingSystem.GetCurrentCraftingProgress();
            }
        }
    }

    private void SubscribeEvents()
    {
        _craftingSystem.OnRecipeUnlocked += OnRecipeUnlocked;
        _craftingSystem.OnItemCrafted += OnItemCrafted;
        _storageSystem.OnStorageUpdated += OnStorageUpdated;
        
        _craftingSystem.OnCraftingStarted += OnCraftingStarted;
        _craftingSystem.OnCraftingCompleted += OnCraftingCompleted;
        
        craftButton.onClick.AddListener(OnCraftButtonClicked);
        
        if (craftAmountInput != null)
        {
            craftAmountInput.onValueChanged.AddListener(OnCraftAmountInputChanged);
        }
    }

    private void UnsubscribeEvents()
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
            craftAmountInput.onValueChanged.RemoveListener(OnCraftAmountInputChanged);
        }
    }

    private void InitializeUI()
    {
        if (craftingProgressBar != null) craftingProgressBar.gameObject.SetActive(false);
        if (placeholderTextObject != null) placeholderTextObject.SetActive(false);
        if (craftingTimeText != null) craftingTimeText.text = "";
        
        if (_craftingSystem != null && _craftingSystem.IsAnyCraftingInProgress())
        {
            _isCrafting = true;
            if (craftingProgressBar != null) craftingProgressBar.gameObject.SetActive(true);
        }
    }

    private void OnRecipeUnlocked(string recipeId) => RefreshRecipeList();

    private void OnItemCrafted(string recipeId, int amount)
    {
        if (_selectedRecipe != null && _selectedRecipe.recipeId == recipeId)
        {
            UpdateDynamicRecipeData();
        }
        UpdateAllButtonCraftableStatus();
        UpdateCraftButtonState();
    }
    
    private void OnStorageUpdated()
    {
        if (_selectedRecipe != null) UpdateDynamicRecipeData();
        UpdateAllButtonCraftableStatus();
        UpdateCraftButtonState();
    }

    private void OnCraftingStarted(string recipeId)
    {
        _isCrafting = true;
        
        if (_selectedRecipe != null && _selectedRecipe.recipeId == recipeId)
        {
            if (craftingProgressBar != null)
            {
                craftingProgressBar.gameObject.SetActive(true);
                craftingProgressBar.value = 0;
            }
        }
        UpdateCraftButtonState();
        UpdateAllButtonCraftableStatus();
    }

    private void OnCraftingCompleted(string recipeId)
    {
        _isCrafting = false;
        
        if (_selectedRecipe != null && _selectedRecipe.recipeId == recipeId)
        {
            if (craftingProgressBar != null)
            {
                craftingProgressBar.gameObject.SetActive(false);
                craftingProgressBar.value = 0;
            }
        }
        UpdateCraftButtonState();
        UpdateAllButtonCraftableStatus();
    }

    private void OnCraftAmountInputChanged(string value)
    {
        if (_selectedRecipe != null)
        {
            UpdateDynamicRecipeData();
            UpdateCraftButtonState();
        }
    }


    private void RefreshRecipeList()
    {
        foreach (Transform child in recipeListContainer) Destroy(child.gameObject);
        _recipeButtons.Clear();
        
        if (_craftingSystem == null) return;
        var unlockedRecipes = _craftingSystem.GetAllUnlockedRecipes();
        
        if (placeholderTextObject != null)
            placeholderTextObject.SetActive(unlockedRecipes.Count == 0);
        
        foreach (var recipe in unlockedRecipes)
        {
            var buttonGO = Instantiate(recipeButtonPrefab, recipeListContainer);
            var buttonUI = buttonGO.GetComponent<CraftingRecipeUIItem>();
            buttonUI.Initialize(recipe, this, _craftingSystem);
            _recipeButtons.Add(buttonUI);
        }
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

        SetupIngredientSlots(recipe);
        
        UpdateDynamicRecipeData();

        if (craftingProgressBar != null)
        {
            bool isCurrentRecipe = _craftingSystem.IsAnyCraftingInProgress() && 
                                   _craftingSystem.GetCurrentCraftingRecipeId() == recipe.recipeId;
            
            craftingProgressBar.gameObject.SetActive(isCurrentRecipe);
            if(isCurrentRecipe) craftingProgressBar.value = _craftingSystem.GetCurrentCraftingProgress();
        }
        
        UpdateCraftButtonState();
    }

    private void SetupIngredientSlots(CraftingRecipe recipe)
    {
        foreach (Transform child in ingredientsContainer) Destroy(child.gameObject);
        _activeIngredientSlots.Clear();

        foreach (var ingredient in recipe.ingredients)
        {
            var slotGO = Instantiate(ingredientSlotPrefab, ingredientsContainer);
            var slotUI = slotGO.GetComponent<IngredientSlotUI>();
            _activeIngredientSlots.Add(slotUI);
        }
    }

    private void UpdateDynamicRecipeData()
    {
        if (_selectedRecipe == null) return;

        int multiplier = GetCraftAmount();

        for (int i = 0; i < _selectedRecipe.ingredients.Count; i++)
        {
            if (i >= _activeIngredientSlots.Count) break;

            var ingredient = _selectedRecipe.ingredients[i];
            var slotUI = _activeIngredientSlots[i];

            Sprite icon = null;
            string name = "";
            int owned = 0;

            if (ingredient.useInventoryItem)
            {
                var itemData = _craftingSystem.GetItemData(ingredient.itemName);
                if (itemData != null) { icon = itemData.icon; name = itemData.displayName; }
                owned = _storageSystem.GetInventoryItemQuantity(ingredient.itemName);
            }
            else
            {
                var resData = _storageSystem.GetResourceData(ingredient.resourceType);
                if (resData != null) { icon = resData.Icon; name = resData.DisplayName; }
                owned = _storageSystem.GetResourceAmount(ingredient.resourceType);
            }

            int totalNeeded = ingredient.amount * multiplier;
            
            slotUI.SetData(icon, name, owned, totalNeeded);
        }

        if (craftingTimeText != null)
        {
            float totalSeconds = _selectedRecipe.craftingTimeInSeconds * multiplier;
            float secondsPerCycle = 4.0f;
            int totalCycles = Mathf.CeilToInt(totalSeconds / secondsPerCycle);
            if (totalCycles < 1) totalCycles = 1;
            craftingTimeText.text = $"{totalCycles} cycles"; 
        }
    }
    
    private int GetCraftAmount()
    {
        if (craftAmountInput == null) return 1;
        if (int.TryParse(craftAmountInput.text, out int amount))
        {
            return amount > 0 ? amount : 1;
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

    public string GetSaveId() => "CraftingPanelUI";

    public JToken CaptureState()
    {
        var recipes = new JArray();
        foreach (var button in _recipeButtons) recipes.Add(button.RecipeId);
        return new JObject() { ["UnlockedRecipes"] = recipes };
    }

    public void RestoreState(JToken state)
    {
        JObject obj = state as JObject;
        if (obj == null) return;
        JArray recipes = obj["UnlockedRecipes"] as JArray;
        
        _craftingSystem = ServiceLocator.GetService<CraftingSystem>();
        if (recipes != null && _craftingSystem != null)
        {
            foreach (var r in recipes) _craftingSystem.UnlockRecipe(r.ToObject<string>());
            RefreshRecipeList();
        }
    }
}