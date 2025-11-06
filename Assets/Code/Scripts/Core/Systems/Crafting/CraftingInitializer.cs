using UnityEngine;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Core.Systems.Storage;

namespace Code.Scripts.Core.Systems.Crafting
{  

    public class CraftingInitializer : MonoBehaviour
    {
        [Header("Base de Datos de Recetas")]
        [SerializeField] private List<CraftingRecipe> allGameRecipes;
    
        [Header("Base de Datos de Items")]
        [SerializeField] private InventoryData itemDatabase;
        
        private CraftingSystem _craftingSystem;

        void Awake()
        {
            _craftingSystem = new CraftingSystem(allGameRecipes, itemDatabase);
        }

        void Start()
        {
            _craftingSystem.InitializeDependencies();
        }
    }
}