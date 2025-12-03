using System;
using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using Code.Scripts.Config;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Systems.Crafting
{
    public class CraftingData
    {
        public string recipeId;
        public float startTime;
        public int amount;
        public int cyclesNeeded;
        public int cyclesCompleted;
    }

    public class CraftingSystem
    {
        private Dictionary<string, CraftingRecipe> _recipeDatabase;
        private HashSet<string> _unlockedRecipeIds;
        private HashSet<string> _viewedRecipeIds;
        private StorageSystem _storageSystem;
        private Dictionary<string, ItemData> _itemDataLookup;

        private IGameTime _gameTime;
        private const float STANDARD_SECONDS_PER_CYCLE = 5.0f;
        
        private CraftingData _currentCraftingData;

        public event Action<string> OnRecipeUnlocked;
        public event Action<string> OnCraftingStarted;
        public event Action<string, float> OnCraftingProgress;
        public event Action<string> OnCraftingCompleted;
        public event Action<string, int> OnItemCrafted;

        public CraftingSystem(List<CraftingRecipe> allRecipes, InventoryData itemDatabase)
        {
            ServiceLocator.RegisterService<CraftingSystem>(this);

            _recipeDatabase = new Dictionary<string, CraftingRecipe>();
            foreach (var recipe in allRecipes)
            {
                if (recipe != null && !_recipeDatabase.ContainsKey(recipe.recipeId))
                {
                    _recipeDatabase.Add(recipe.recipeId, recipe);
                }
            }

            _unlockedRecipeIds = new HashSet<string>();
            _viewedRecipeIds = new HashSet<string>();

            _itemDataLookup = new Dictionary<string, ItemData>();
            if (itemDatabase != null)
            {
                foreach (var item in itemDatabase.items)
                {
                    if (item.itemData != null && !_itemDataLookup.ContainsKey(item.itemData.itemName))
                    {
                        _itemDataLookup.Add(item.itemData.itemName, item.itemData);
                    }
                }
            }
        }

        public void InitializeDependencies()
        {
            _storageSystem = WorldManager.Instance.Player.StorageSystem;
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnCycleCompleted += OnCycleCompleted;
        }

        private void OnDestroy()
        {
            if (_gameTime != null)
            {
                _gameTime.OnCycleCompleted -= OnCycleCompleted;
            }
        }
        private void OnCycleCompleted(int currentCycle)
        {
            if (!IsAnyCraftingInProgress()) return;

            _currentCraftingData.cyclesCompleted++;
            float progress = 0f;
            if (_currentCraftingData.cyclesNeeded > 0)
            {
                progress = Mathf.Clamp01((float)_currentCraftingData.cyclesCompleted / _currentCraftingData.cyclesNeeded);
            }
            
            OnCraftingProgress?.Invoke(_currentCraftingData.recipeId, progress);

            if (_currentCraftingData.cyclesCompleted >= _currentCraftingData.cyclesNeeded)
            {
                CompleteCrafting();
            }
        }

        public bool HasRecipeBeenViewed(string recipeId)
        {
            return _viewedRecipeIds.Contains(recipeId);
        }

        public void MarkRecipeAsViewed(string recipeId)
        {
            if (_unlockedRecipeIds.Contains(recipeId) && !_viewedRecipeIds.Contains(recipeId))
            {
                _viewedRecipeIds.Add(recipeId);
            }
        }

        public void UnlockRecipe(string recipeId)
        {
            if (!_recipeDatabase.ContainsKey(recipeId) || _unlockedRecipeIds.Contains(recipeId))
                return;

            _unlockedRecipeIds.Add(recipeId);
            OnRecipeUnlocked?.Invoke(recipeId);
        }

        public bool IsRecipeUnlocked(string recipeId)
        {
            return _unlockedRecipeIds.Contains(recipeId);
        }

        public bool IsAnyCraftingInProgress()
        {
            return _currentCraftingData != null;
        }

        public bool CanCraft(string recipeId, int amount = 1)
        {
            if (!IsRecipeUnlocked(recipeId)) return false;
            if (IsAnyCraftingInProgress()) return false;
            if (_storageSystem == null) return false;

            var recipe = _recipeDatabase[recipeId];
            if (recipe == null) return false;

            foreach (var ingredient in recipe.ingredients)
            {
                int requiredAmount = ingredient.amount * amount;
                if (ingredient.useInventoryItem)
                {
                    if (!_storageSystem.HasInventoryItem(ingredient.itemName, requiredAmount))
                        return false;
                }
                else
                {
                    if (!_storageSystem.HasResource(ingredient.resourceType, requiredAmount))
                        return false;
                }
            }

            var output = recipe.output;
            int outputAmount = output.amount * amount;

            if (!_itemDataLookup.TryGetValue(output.itemName, out ItemData itemData))
                return false;

            int maxStack = itemData.maxStack;
            int currentAmount = _storageSystem.GetInventoryItemQuantity(output.itemName);

            if (currentAmount + outputAmount > maxStack)
                return false;

            return true;
        }

        public bool Craft(string recipeId, int amount = 1)
        {
            if (!CanCraft(recipeId, amount)) return false;

            var recipe = _recipeDatabase[recipeId];

            foreach (var ingredient in recipe.ingredients)
            {
                int requiredAmount = ingredient.amount * amount;
                if (ingredient.useInventoryItem)
                {
                    _storageSystem.ConsumeInventoryItem(ingredient.itemName, requiredAmount);
                }
                else
                {
                    _storageSystem.ConsumeResource(ingredient.resourceType, requiredAmount);
                }
            }

            _currentCraftingData = new CraftingData
            {
                recipeId = recipeId,
                startTime = _gameTime.GameTime,
                amount = amount,
                cyclesNeeded = CalculateCyclesNeeded(recipe.craftingTimeInSeconds * amount),
                cyclesCompleted = 0
            };

            OnCraftingStarted?.Invoke(recipeId);
            return true;
        }
        private int CalculateCyclesNeeded(float craftingTimeInSeconds)
        {
            int cycles = Mathf.CeilToInt(craftingTimeInSeconds / STANDARD_SECONDS_PER_CYCLE);
            return Mathf.Max(1, cycles);
        }

        private void CompleteCrafting()
        {
            string recipeId = _currentCraftingData.recipeId;
            int amount = _currentCraftingData.amount;
            var recipe = _recipeDatabase[recipeId];

            var output = recipe.output;
            _storageSystem.AddInventoryItem(output.itemName, output.amount * amount);

            var completedData = _currentCraftingData;
            _currentCraftingData = null;

            OnItemCrafted?.Invoke(recipeId, output.amount * amount);
            OnCraftingCompleted?.Invoke(recipeId);

            ItemData craftedItemData = GetItemData(output.itemName);
            if (craftedItemData != null && craftedItemData.itemToUnlock != null)
            {
                if (craftedItemData.itemToUnlock is PlanetDataSO)
                {
                    ResearchEvents.OnNewPlanetResearched?.Invoke(craftedItemData.itemToUnlock as PlanetDataSO);
                }
            }
            CraftingEvents.OnItemCrafted?.Invoke(GetItemData(output.itemName));
            NotificationManager.Instance.CreateNotification($"Crafting: {craftedItemData.itemName} completed", NotificationType.Info);
        }

        public CraftingRecipe GetRecipe(string recipeId)
        {
            return _recipeDatabase.GetValueOrDefault(recipeId);
        }

        public ItemData GetItemData(string itemName)
        {
            return _itemDataLookup.GetValueOrDefault(itemName);
        }

        public List<CraftingRecipe> GetAllUnlockedRecipes()
        {
            List<CraftingRecipe> unlockedList = new List<CraftingRecipe>();
            foreach (var id in _unlockedRecipeIds)
            {
                unlockedList.Add(_recipeDatabase[id]);
            }
            return unlockedList;
        }

        public string GetCurrentCraftingRecipeId()
        {
            return IsAnyCraftingInProgress() ? _currentCraftingData.recipeId : null;
        }

        public float GetCurrentCraftingProgress()
        {
            if (!IsAnyCraftingInProgress()) return 0f;
            if (_currentCraftingData.cyclesNeeded == 0) return 0f; // Seguridad extra

            return Mathf.Clamp01((float)_currentCraftingData.cyclesCompleted / _currentCraftingData.cyclesNeeded);
        }

        public int GetCurrentCraftingAmount()
        {
            return IsAnyCraftingInProgress() ? _currentCraftingData.amount : 0;
        }
    }
}