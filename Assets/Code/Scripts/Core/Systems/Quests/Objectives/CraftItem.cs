using UnityEngine;
using Code.Scripts.Core.Systems.Crafting; // Para CraftingSystem
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Quests.Objectives
{
    [System.Serializable]
    public class CraftItem : QuestObjective
    {
        public string requiredRecipeId;
        
        private CraftingSystem _craftingSystem;

        public override void Initialize()
        {
            isCompleted = false;
        }

        public override void RegisterEvents()
        {
            try
            {
                _craftingSystem = ServiceLocator.GetService<CraftingSystem>();
                _craftingSystem.OnItemCrafted += HandleItemCrafted;
            }
            catch (System.Exception e)
            {
            }
        }

        public override void UnregisterEvents()
        {
            if (_craftingSystem != null)
            {
                _craftingSystem.OnItemCrafted -= HandleItemCrafted;
            }
        }
        
        private void HandleItemCrafted(string recipeId, int amount)
        {
            if (isCompleted) return;

            // ¡Comprobamos si la receta crafteada es la que buscábamos!
            if (recipeId == requiredRecipeId)
            {
                isCompleted = true;
                UnregisterEvents();
            }
        }

        public override void CheckCompletion()
        {
        }
    }
}