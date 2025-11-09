using System;
using UnityEngine;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_UnlockCraftingRecipe : AbstractResearchReward
    {
        public string recipeId;

        public override void ApplyReward()
        {
            var craftingSystem = ServiceLocator.GetService<CraftingSystem>();
            if (craftingSystem != null)
            {
                craftingSystem.UnlockRecipe(recipeId);
            }
            else
            {
                Debug.LogError($"No se pudo encontrar el CraftingSystem para desbloquear: {recipeId}");
            }
        }
        
        public override string GetDescription() => $"Desbloquear receta: {recipeId}";
    }
}