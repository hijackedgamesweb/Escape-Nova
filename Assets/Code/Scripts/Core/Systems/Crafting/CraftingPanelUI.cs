using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;

public class CraftingPanelUI : MonoBehaviour
{
    [Header("Sistemas")]
    private CraftingSystem _craftingSystem;
    private StorageSystem _storageSystem;

    [Header("Prefabs")]
    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private GameObject ingredientSlotPrefab;

    [Header("Contenedores")]
    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private Transform ingredientsContainer;
    
    [Header("Panel de Detalles")]
    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private Button craftButton;
    [SerializeField] private TMP_InputField craftAmountInput;
    
    [Header("Progreso de Crafteo")]
    [SerializeField] private Slider craftingProgressBar;

    private List<RecipeButtonUI> _recipeButtons = new List<RecipeButtonUI>();
    private CraftingRecipe _selectedRecipe;

    void Start()
    {
        _craftingSystem = ServiceLocator.GetService<CraftingSystem>();
        _storageSystem = ServiceLocator.GetService<StorageSystem>();
        
        if (_craftingSystem == null || _storageSystem == null)
        {
            Debug.LogError("¡No se pudieron encontrar los sistemas de Crafting o Storage!");
            return;
        }

        _craftingSystem.OnRecipeUnlocked += OnRecipeUnlocked;
        _craftingSystem.OnItemCrafted += OnItemCrafted;
        _storageSystem.OnStorageUpdated += OnStorageUpdated;
        
        _craftingSystem.OnCraftingStarted += OnCraftingStarted;
        _craftingSystem.OnCraftingProgress += OnCraftingProgress;
        _craftingSystem.OnCraftingCompleted += OnCraftingCompleted;
        
        craftButton.onClick.AddListener(OnCraftButtonClicked);

        detailsPanel.SetActive(false);
        
        // ¡NUEVO! Ocultar la barra de progreso al inicio
        if (craftingProgressBar != null)
        {
            craftingProgressBar.gameObject.SetActive(false);
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
        if (craftingProgressBar != null)
        {
            craftingProgressBar.gameObject.SetActive(true);
            craftingProgressBar.value = 0;
        }
        UpdateCraftButtonState();
    }

    private void OnCraftingProgress(string recipeId, float progress)
    {
        if (craftingProgressBar != null)
        {
            craftingProgressBar.value = progress;
        }
    }

    private void OnCraftingCompleted(string recipeId)
    {
        if (craftingProgressBar != null)
        {
            craftingProgressBar.gameObject.SetActive(false);
        }
        UpdateCraftButtonState();
    }

    private void RefreshRecipeList()
    {
        foreach (Transform child in recipeListContainer) Destroy(child.gameObject);
        _recipeButtons.Clear();

        var unlockedRecipes = _craftingSystem.GetAllUnlockedRecipes();
        
        foreach (var recipe in unlockedRecipes)
        {
            var buttonGO = Instantiate(recipeButtonPrefab, recipeListContainer);
            var buttonUI = buttonGO.GetComponent<RecipeButtonUI>();
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
        detailsPanel.SetActive(true);

        var outputItemData = _craftingSystem.GetItemData(recipe.output.itemName);
        if (outputItemData != null)
        {
            detailIcon.sprite = outputItemData.icon;
            detailName.text = outputItemData.displayName;
            detailDescription.text = outputItemData.description;
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
                if (itemData != null)
                {
                    ingredientIcon = itemData.icon;
                    ingredientName = itemData.displayName;
                }
                amountOwned = _storageSystem.GetInventoryItemQuantity(ingredient.itemName);
            }
            else
            {
                var resourceData = _storageSystem.GetResourceData(ingredient.resourceType);
                if (resourceData != null)
                {
                    ingredientIcon = resourceData.Icon;
                    ingredientName = resourceData.DisplayName;
                }
                amountOwned = _storageSystem.GetResourceAmount(ingredient.resourceType);
            }
            
            slotUI.SetData(ingredientIcon, ingredientName, amountOwned, ingredient.amount);
        }
        
        UpdateCraftButtonState();
        
        if (!_craftingSystem.IsAnyCraftingInProgress())
        {
            if (craftingProgressBar != null)
                craftingProgressBar.gameObject.SetActive(false);
        }
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