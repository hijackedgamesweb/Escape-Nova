using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.Scripts.Core.Systems.Crafting; // Para CraftingRecipe y CraftingSystem
using Code.Scripts.Core.Systems.Storage; // Para StorageSystem
using Code.Scripts.Patterns.ServiceLocator; // Para ServiceLocator
using UnityEngine.EventSystems; // Para IPointerClickHandler

// ¡Asegúrate de que el namespace coincide con el 'using' del Paso 1!
namespace Code.Scripts.UI.Crafting 
{
    // Este script va en tu prefab 'recipeButtonPrefab'
    public class CraftingRecipeUIItem : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private GameObject selectionHighlight; // Borde para "seleccionado"
        [SerializeField] private GameObject canCraftHighlight; // Borde verde para "crafteable"

        private CraftingRecipe _recipe;
        private CraftingPanelUI _panelUI;
        private CraftingSystem _craftingSystem;
        private StorageSystem _storageSystem;

        // Esto es lo que llama el CraftingPanelUI
        public void Initialize(CraftingRecipe recipe, CraftingPanelUI panel, CraftingSystem system)
        {
            _recipe = recipe;
            _panelUI = panel;
            _craftingSystem = system;
            
            // Obtenemos el storage para futuras comprobaciones
            _storageSystem = ServiceLocator.GetService<StorageSystem>(); 

            // Rellenamos el botón con los datos de la RECETA
            // Usamos el 'displayName' y 'icon' del ScriptableObject 'CraftingRecipe'
            itemName.text = _recipe.displayName;
            itemIcon.sprite = _recipe.icon; 

            // (Opcional: si quieres el icono del item de *salida* en lugar del de la receta)
            /*
            var outputItemData = _craftingSystem.GetItemData(_recipe.output.itemName);
            if (outputItemData != null)
            {
               itemName.text = outputItemData.displayName;
               itemIcon.sprite = outputItemData.icon;
            }
            */

            SetSelected(false);
            UpdateCraftableStatus();
        }

        // Esto es lo que llama el CraftingPanelUI para refrescar
        public void UpdateCraftableStatus()
        {
            if (_craftingSystem == null || _recipe == null) return;

            // Preguntamos al sistema si podemos craftear esto
            bool canCraft = _craftingSystem.CanCraft(_recipe.recipeId);

            if (canCraftHighlight != null)
            {
                canCraftHighlight.SetActive(canCraft);
            }
        }

        // Para el borde de "seleccionado"
        public void SetSelected(bool isSelected)
        {
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(isSelected);
            }
        }

        // Cuando el jugador hace clic en este botón
        public void OnPointerClick(PointerEventData eventData)
        {
            // ¡Avisamos al panel principal!
            _panelUI.SelectRecipe(_recipe);
        }
    }
}