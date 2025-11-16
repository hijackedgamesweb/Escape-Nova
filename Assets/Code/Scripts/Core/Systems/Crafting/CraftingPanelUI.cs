using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Crafting;

public class CraftingPanelUI : MonoBehaviour
{
    private CraftingSystem _craftingSystem;
    private StorageSystem _storageSystem;

    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private GameObject ingredientSlotPrefab;

    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private Transform ingredientsContainer;
    
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailTimeText;
    [SerializeField] private Button craftButton;
    [SerializeField] private TMP_InputField craftAmountInput;
    
    [SerializeField] private Slider craftingProgressBar;
    
    [SerializeField] private GameObject placeholderTextObject;

    private List<CraftingRecipeUIItem> _recipeButtons = new List<CraftingRecipeUIItem>();
    private CraftingRecipe _selectedRecipe;
    
    void Start()
    {
        _craftingSystem = ServiceLocator.GetService<CraftingSystem>();
        _storageSystem = ServiceLocator.GetService<StorageSystem>();
        
        if (_craftingSystem == null || _storageSystem == null)
        {
            return;
        }

        _craftingSystem.OnRecipeUnlocked += OnRecipeUnlocked;
        _craftingSystem.OnItemCrafted += OnItemCrafted;
        _storageSystem.OnStorageUpdated += OnStorageUpdated;
        
        _craftingSystem.OnCraftingStarted += OnCraftingStarted;
        _craftingSystem.OnCraftingProgress += OnCraftingProgress;
        _craftingSystem.OnCraftingCompleted += OnCraftingCompleted;
        
        craftButton.onClick.AddListener(OnCraftButtonClicked);
        
        if (craftingProgressBar != null)
        {
            craftingProgressBar.gameObject.SetActive(false);
        }

        if (placeholderTextObject != null)
        {
            placeholderTextObject.SetActive(false);
        }
            
        
        RefreshRecipeList();
    }

    private void OnDestroy()
    {
        if (_craftingSystem != null)
        {
            _craftingSystem.OnRecipeUnlocked -= OnRecipeUnlocked;
            _craftingSystem.OnItemCrafted -= OnItemCrafted;
            _craftingSystem.OnCraftingStarted -= OnCraftingStarted;
            _craftingSystem.OnCraftingProgress -= OnCraftingProgress;
            _craftingSystem.OnCraftingCompleted -= OnCraftingCompleted;
        }
        if (_storageSystem != null)
        {
            _storageSystem.OnStorageUpdated -= OnStorageUpdated;
        }
    }
    
    private void OnRecipeUnlocked(string recipeId)
    {
        RefreshRecipeList();
    }

    private void OnItemCrafted(string recipeId, int amount)
    {
        if (_selectedRecipe != null && _selectedRecipe.recipeId == recipeId)
        {
            SelectRecipe(_selectedRecipe);
        }
        UpdateAllButtonCraftableStatus();
    }
    
    private void OnStorageUpdated()
    {
        if (_selectedRecipe != null)
        {
            SelectRecipe(_selectedRecipe); 
        }
        UpdateAllButtonCraftableStatus();
    }

    
    private void OnCraftingStarted(string recipeId)
    {
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

    private void OnCraftingProgress(string recipeId, float progress)
    {
        if (_selectedRecipe != null && _selectedRecipe.recipeId == recipeId)
        {
            if (craftingProgressBar != null)
            {                
                craftingProgressBar.value = progress;
            }
        }
    }

    private void OnCraftingCompleted(string recipeId)
    {
        if (_selectedRecipe != null && _selectedRecipe.recipeId == recipeId)
        {
            if (craftingProgressBar != null)
            {
                craftingProgressBar.gameObject.SetActive(false);
            }
        }
        UpdateCraftButtonState();
        UpdateAllButtonCraftableStatus();
    }


    private void RefreshRecipeList()
    {
        foreach (Transform child in recipeListContainer) Destroy(child.gameObject);
        _recipeButtons.Clear();
        if (_craftingSystem == null) return;
        var unlockedRecipes = _craftingSystem.GetAllUnlockedRecipes();
        
        if (placeholderTextObject != null)
        {
            placeholderTextObject.SetActive(unlockedRecipes.Count == 0);
        }
        
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
        foreach(var button in _recipeButtons)
        {
            button.UpdateCraftableStatus();
        }
    }

    
    public void SelectRecipe(CraftingRecipe recipe)
    {
        _selectedRecipe = recipe;
        
        CraftingRecipeUIItem selectedItem = null;

        foreach (var button in _recipeButtons)
        {
            button.SetSelected(false);
            
            if (button.RecipeId == recipe.recipeId)
            {
                selectedItem = button;
            }
        }

        if (selectedItem != null)
        {
            selectedItem.SetSelected(true);
        }
        
        if (placeholderTextObject != null)
        {
            placeholderTextObject.SetActive(false);
        }

        var outputItemData = _craftingSystem.GetItemData(recipe.output.itemName);
        if (outputItemData != null)
        {
            detailIcon.sprite = outputItemData.icon;
            detailName.text = outputItemData.displayName;
            detailDescription.text = outputItemData.description;
        }
        
        if (detailTimeText != null)
        {
            const float secondsPerCycle = 5.0f;
            float cyclesAsFloat = recipe.craftingTimeInSeconds / secondsPerCycle;
            int displayCycles = Mathf.CeilToInt(cyclesAsFloat);
            
            detailTimeText.text = $"Time: {displayCycles} cicles";
        }

        foreach (Transform child in ingredientsContainer) Destroy(child.gameObject);

        foreach (var ingredient in recipe.ingredients)
        {
            var slotGO = Instantiate(ingredientSlotPrefab, ingredientsContainer);
            var slotUI = slotGO.GetComponent<IngredientSlotUI>();
            
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
            
            slotUI.SetData(ingredientIcon, ingredientName, amountOwned, ingredient.amount);
        }
        
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
        
        UpdateCraftButtonState();
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
            craftButton.interactable = false;
            return;
        }
        
        if (_craftingSystem.IsAnyCraftingInProgress())
        {
            craftButton.interactable = false;
            return;
        }
        
        int amountToCraft = GetCraftAmount(); 
        craftButton.interactable = _craftingSystem.CanCraft(_selectedRecipe.recipeId, amountToCraft);
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