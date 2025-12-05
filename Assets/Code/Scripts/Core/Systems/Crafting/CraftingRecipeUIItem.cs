using Code.Scripts.Core.Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine.EventSystems;

namespace Code.Scripts.UI.Crafting 
{
    public class CraftingRecipeUIItem : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private GameObject selectionHighlight;
        [SerializeField] private Image canCraftHighlight;

        private CraftingRecipe _recipe;
        private CraftingPanelUI _panelUI;
        private CraftingSystem _craftingSystem;
        private StorageSystem _storageSystem;

        public string RecipeId { get; private set; }

        public void Initialize(CraftingRecipe recipe, CraftingPanelUI panel, CraftingSystem system)
        {
            _recipe = recipe;
            _panelUI = panel;
            _craftingSystem = system;
            
            _storageSystem = WorldManager.Instance.Player.StorageSystem;

            itemName.text = _recipe.displayName;
            itemIcon.sprite = _recipe.icon; 
            RecipeId = _recipe.recipeId;

            SetSelected(false);
            UpdateCraftableStatus();
        }

        public void UpdateCraftableStatus()
        {
            if (_craftingSystem == null || _recipe == null) return;

            bool canCraft = _craftingSystem.CanCraft(_recipe.recipeId);

            if (canCraft)
            {
                canCraftHighlight.color = new Color(0f, 0f, 0f, 0f); 
            }
            else
            {
                canCraftHighlight.color = new Color(0f, 0f, 0f, 0.2f);
            }
        }

        public void SetSelected(bool isSelected)
        {
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(isSelected);
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _panelUI.SelectRecipe(_recipe);
        }
    }
}