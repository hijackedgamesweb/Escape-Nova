using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Crafting
{
    [System.Serializable]
    public class CraftingIngredient
    {
        public ResourceType resourceType;
        public int amount;
        public string itemName;
        public bool useInventoryItem;
    }
    
    [System.Serializable]
    public class CraftingOutput
    {
        public string itemName;
        public int amount;
    }
    
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Game/Crafting/Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [Header("Info General")]
        public string recipeId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Costes y Resultados")]
        public List<CraftingIngredient> ingredients;
        public CraftingOutput output;

        [Header("Requisitos")]
        public float craftingTimeInSeconds = 1f;
    }
}