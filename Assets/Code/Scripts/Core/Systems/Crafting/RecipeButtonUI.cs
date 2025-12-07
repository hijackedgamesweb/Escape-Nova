using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.UI.Crafting;


namespace Code.Scripts.Core.Systems.Crafting
{
    public class RecipeButtonUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private GameObject notificationPip;
        [SerializeField] private GameObject canCraftOverlay;
    
        private CraftingRecipe _recipe;
        private CraftingPanelUI _manager;
        private CraftingSystem _craftingSystem;

        public void Initialize(CraftingRecipe recipe, CraftingPanelUI manager, CraftingSystem system)
        {
            _recipe = recipe;
            _manager = manager;
            _craftingSystem = system;
        
            var outputItemData = _craftingSystem.GetItemData(recipe.output.itemName);
            if (outputItemData != null)
            {
                icon.sprite = outputItemData.icon;
            }
        
            GetComponent<Button>().onClick.AddListener(OnSelect);
        
            if (notificationPip != null)
            {
                if (_craftingSystem.HasRecipeBeenViewed(_recipe.recipeId))
                {
                    notificationPip.SetActive(false);
                }
                else
                {
                    notificationPip.SetActive(true);
                }
            }
        
            UpdateCraftableStatus();
        }

        public void UpdateCraftableStatus()
        {
            if (canCraftOverlay != null)
            {
                bool canCraft = _craftingSystem.CanCraft(_recipe.recipeId);
                canCraftOverlay.SetActive(canCraft);
            }
        }

        private void OnSelect()
        {
            _craftingSystem.MarkRecipeAsViewed(_recipe.recipeId);
        
            if (notificationPip != null)
            {
                notificationPip.SetActive(false);
            }
            _manager.SelectRecipe(_recipe);
        }
    }
}