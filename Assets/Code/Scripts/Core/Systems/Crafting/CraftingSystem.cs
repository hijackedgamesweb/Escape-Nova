using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Time;

namespace Code.Scripts.Core.Systems.Crafting
{
    public class CraftingData
    {
        public string recipeId;
        public float startTime;
        public int amount;
    }

    public class CraftingSystem
    {
        private Dictionary<string, CraftingRecipe> _recipeDatabase;
        private HashSet<string> _unlockedRecipeIds;
        private HashSet<string> _viewedRecipeIds;
        private StorageSystem _storageSystem;
        private Dictionary<string, ItemData> _itemDataLookup;
        
        private TimeScheduler _timeScheduler;
        private IGameTime _gameTime;
        private ITimerHandle _currentCraftingTimer;
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
        
        public bool HasRecipeBeenViewed(string recipeId)
        {
            return _viewedRecipeIds.Contains(recipeId);
        }

        public void MarkRecipeAsViewed(string recipeId)
        {
            if (_unlockedRecipeIds.Contains(recipeId) && !_viewedRecipeIds.Contains(recipeId))
            {
                _viewedRecipeIds.Add(recipeId);
                Debug.Log($"Receta marcada como vista: {recipeId}");
            }
        }

        public void InitializeDependencies()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            _timeScheduler = ServiceLocator.GetService<TimeScheduler>();
            _gameTime = ServiceLocator.GetService<IGameTime>();
        }

        public void UnlockRecipe(string recipeId)
        {
            if (!_recipeDatabase.ContainsKey(recipeId))
            {
                Debug.LogWarning($"Se intentó desbloquear una receta que no existe: {recipeId}");
                return;
            }
            if (_unlockedRecipeIds.Contains(recipeId))
            {
                Debug.Log($"La receta {recipeId} ya estaba desbloqueada.");
                return;
            }
            _unlockedRecipeIds.Add(recipeId);
            OnRecipeUnlocked?.Invoke(recipeId);
            Debug.Log($"Receta desbloqueada: {recipeId}");
        }

        public bool IsRecipeUnlocked(string recipeId)
        {
            return _unlockedRecipeIds.Contains(recipeId);
        }

        public bool IsAnyCraftingInProgress()
        {
            return _currentCraftingTimer != null && _currentCraftingData != null;
        }

        public bool CanCraft(string recipeId, int amount = 1)
        {
            if (!IsRecipeUnlocked(recipeId)) return false;
            if (IsAnyCraftingInProgress())
            {
                Debug.Log("CanCraft es 'false' porque ya hay un crafteo en progreso.");
                return false;
            }
            if (_storageSystem == null) return false;

            var recipe = _recipeDatabase[recipeId];
            if (recipe == null) return false;
    
            // --- DEBUG DE INGREDIENTES ---
            foreach (var ingredient in recipe.ingredients)
            {
                int requiredAmount = ingredient.amount * amount;
                if (ingredient.useInventoryItem)
                {
                    if (!_storageSystem.HasInventoryItem(ingredient.itemName, requiredAmount))
                    {
                        Debug.Log($"CanCraft es 'false' por falta de item-ingrediente: {ingredient.itemName}");
                        return false;
                    }
                }
                else
                {
                    if (!_storageSystem.HasResource(ingredient.resourceType, requiredAmount))
                    {
                        Debug.Log($"CanCraft es 'false' por falta de recurso-ingrediente: {ingredient.resourceType}");
                        return false;
                    }
                }
            }
            // ----------------------------

            var output = recipe.output;
            int outputAmount = output.amount * amount;

            if (!_itemDataLookup.TryGetValue(output.itemName, out ItemData itemData))
            {
                Debug.LogError($"CanCraft falló: No se encontró ItemData para {output.itemName} en _itemDataLookup"); 
                return false; 
            }
    
            int maxStack = itemData.maxStack;
            int currentAmount = _storageSystem.GetInventoryItemQuantity(output.itemName);

            // --- DEBUG DE MAXSTACK ---
            if (currentAmount + outputAmount > maxStack)
            {
                Debug.Log($"CanCraft es 'false' por MAXSTACK. current({currentAmount}) + output({outputAmount}) > max({maxStack})");
                return false; 
            }
            // -------------------------

            Debug.Log("CanCraft es 'true'. ¡Todo en orden!");
            return true;
        }

        public bool Craft(string recipeId, int amount = 1)
        {
            if (!CanCraft(recipeId, amount))
            {
                Debug.LogWarning($"Intento de craftear {recipeId} sin cumplir requisitos.");
                return false;
            }

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
                amount = amount
            };
            
            if (_currentCraftingTimer != null)
            {
                _currentCraftingTimer.Cancel();
            }

            const float UPDATE_INTERVAL = 0.5f; 
            Action updateAction = () => UpdateCraftingProgress(UPDATE_INTERVAL);
            _currentCraftingTimer = _timeScheduler.ScheduleRepeating(UPDATE_INTERVAL, updateAction);

            Debug.Log($"Se ha iniciado el crafteo de: {recipe.displayName}");
            OnCraftingStarted?.Invoke(recipeId);
            return true;
        }

        private void UpdateCraftingProgress(float deltaTime)
        {
            if (!IsAnyCraftingInProgress())
            {
                _currentCraftingTimer?.Cancel();
                _currentCraftingTimer = null;
                return;
            }

            var recipe = _recipeDatabase[_currentCraftingData.recipeId];
            float timeElapsed = _gameTime.GameTime - _currentCraftingData.startTime;
    
            float craftTime = recipe.craftingTimeInSeconds * _currentCraftingData.amount; 

            if (timeElapsed >= craftTime)
            {
                OnCraftingProgress?.Invoke(_currentCraftingData.recipeId, 1f);
                CompleteCrafting();
            }
            else
            {
                float progress = Mathf.Clamp01(timeElapsed / craftTime);
                OnCraftingProgress?.Invoke(_currentCraftingData.recipeId, progress);
            }
        }
        
        private void CompleteCrafting()
        {
            string recipeId = _currentCraftingData.recipeId;
            int amount = _currentCraftingData.amount;
            var recipe = _recipeDatabase[recipeId];

            var output = recipe.output;
            _storageSystem.AddInventoryItem(output.itemName, output.amount * amount); 

            _currentCraftingTimer?.Cancel();
            _currentCraftingTimer = null;
            _currentCraftingData = null;

            Debug.Log($"Se ha completado el crafteo de: {recipe.displayName}!");
            OnItemCrafted?.Invoke(recipeId, output.amount * amount);
            OnCraftingCompleted?.Invoke(recipeId);
            CraftingEvents.OnItemCrafted?.Invoke(GetItemData(output.itemName));
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
            if (!IsAnyCraftingInProgress())
            {
                return null;
            }
            return _currentCraftingData.recipeId;
        }

        public float GetCurrentCraftingProgress()
        {
            if (!IsAnyCraftingInProgress())
            {
                return 0f;
            }

            var recipe = _recipeDatabase[_currentCraftingData.recipeId];
            float timeElapsed = _gameTime.GameTime - _currentCraftingData.startTime;
            float craftTime = recipe.craftingTimeInSeconds * _currentCraftingData.amount;

            return Mathf.Clamp01(timeElapsed / craftTime);
        }

        public int GetCurrentCraftingAmount()
        {
            if (!IsAnyCraftingInProgress())
            {
                return 0;
            }
            return _currentCraftingData.amount;
        }
    }
}