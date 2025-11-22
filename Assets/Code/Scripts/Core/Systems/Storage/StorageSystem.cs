using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;
using ResourceType = Code.Scripts.Core.Systems.Resources.ResourceType;


namespace Code.Scripts.Core.Systems.Storage
{
    public class StorageSystem
    {
        private List<ResourceData> _resourceDataList;
        private InventoryData _inventoryData;
        
        private Dictionary<ResourceType, ResourceData> _resourceDatabase;
        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();
        private Dictionary<string, int> _inventoryItems = new Dictionary<string, int>();
        
        private Dictionary<string, ItemData> _itemDatabase;
        public event Action<ResourceType, int> OnResourceChanged;
        public event Action OnStorageUpdated;
        
        public StorageSystem(List<ResourceData> resourceDataList, InventoryData inventoryData)
        {
            _resourceDataList = resourceDataList;
            _inventoryData = inventoryData;
            Initialize();
        }
        
        private void Initialize()
        {
            InitializeResourceDatabase();
            InitializeInventoryItems();
            ServiceLocator.RegisterService<StorageSystem>(this);
        }

        public void AddMaxCapacity(ResourceType type, int additionalCapacity)
        {
            if (_resourceDatabase.TryGetValue(type, out ResourceData data))
            {
                data.MaxStack += additionalCapacity;
            }
            else
            {
            }
        }
        
        public void InitializeInventoryItems()
        {
            _inventoryItems.Clear();

            _itemDatabase = new Dictionary<string, ItemData>();
            
            if (_inventoryData == null || _inventoryData.items == null)
            {
                return;
            }

            foreach (var item in _inventoryData.items)
            {
                if (item.itemData != null && !string.IsNullOrEmpty(item.itemData.itemName))
                {
                    // ¡Clave! Añadimos el item al diccionario con su CANTIDAD INICIAL
                    _inventoryItems[item.itemData.itemName] = item.quantity;
                    if (!_itemDatabase.ContainsKey(item.itemData.itemName))
                    {
                        _itemDatabase.Add(item.itemData.itemName, item.itemData);
                    }
                }
            }
        }
        
        public bool HasInventoryItem(string itemName, int quantity)
        {
            return _inventoryItems.ContainsKey(itemName) && _inventoryItems[itemName] >= quantity;
        }
        
        public bool ConsumeInventoryItem(string itemName, int quantity)
        {
            if (!HasInventoryItem(itemName, quantity))
                return false;
        
            _inventoryItems[itemName] -= quantity;
            OnStorageUpdated?.Invoke();
            return true;
        }
        
        public bool AddInventoryItem(string itemName, int quantity)
        {
            if (!_inventoryItems.ContainsKey(itemName))
            {
                return false;
            }
            if (!_itemDatabase.TryGetValue(itemName, out ItemData data))
            {
                return false;
            }
            int maxStack = data.maxStack; 
            int currentAmount = _inventoryItems[itemName];
            int newAmount = currentAmount + quantity;

            if (newAmount >= maxStack)
            {
                _inventoryItems[itemName] = maxStack;
            }
            else
            {
                _inventoryItems[itemName] = newAmount;
            }

            OnStorageUpdated?.Invoke();
            return true;
        }
        
        public int GetInventoryItemQuantity(string itemName)
        {
            return _inventoryItems.GetValueOrDefault(itemName, 0);
        }
        
        public Dictionary<string, int> GetInventoryItems()
        {
            return new Dictionary<string, int>(_inventoryItems);
        }
        
        public List<InventoryItem> GetInventoryItemList()
        {
            List<InventoryItem> itemList = new List<InventoryItem>();
            foreach (var kvp in _inventoryItems)
            {
                if (_itemDatabase.TryGetValue(kvp.Key, out ItemData data))
                {
                    itemList.Add(new InventoryItem
                    {
                        itemData = data,
                        quantity = kvp.Value
                    });
                }
            }
            return itemList;
        }

        public ItemData GetItemData(string itemName)
        {
            if (_itemDatabase.TryGetValue(itemName, out ItemData data))
            {
                return data;
            }
            return null;
        }
        
        private void InitializeResourceDatabase()
        {
            _resourceDatabase = new Dictionary<ResourceType, ResourceData>();
            
            if (_resourceDataList != null && _resourceDataList.Count > 0)
            {
                foreach (var resource in _resourceDataList)
                {
                    if (resource != null)
                    {
                        _resourceDatabase[resource.Type] = resource;
                        _resources[resource.Type] = 0;
                    }
                }
            }
            else
            {
            }
            
        }
        
        public bool AddResource(ResourceType type, int amount)
        {
            if (!_resourceDatabase.ContainsKey(type))
            {
                return false;
            }
            if (!_resources.ContainsKey(type))
                _resources[type] = 0;
            var maxStack = _resourceDatabase[type].MaxStack;
            var newAmount = _resources[type] + amount;
            if (newAmount > maxStack)
            {
                _resources[type] = maxStack;
            }
            else
            {
                _resources[type] = newAmount;
            }
            
            OnResourceChanged?.Invoke(type, _resources[type]);
            OnStorageUpdated?.Invoke();
            return true;
        }
        public bool ConsumeResource(ResourceType type, int amount)
        {
            if (!HasResource(type, amount))
                return false;
                
            _resources[type] -= amount;
            OnResourceChanged?.Invoke(type, _resources[type]);
            OnStorageUpdated?.Invoke();
            return true;
        }
        
        public bool HasResource(ResourceType type, int amount)
        {
            return _resources.ContainsKey(type) && _resources[type] >= amount;
        }
        public int GetResourceAmount(ResourceType type)
        {
            return _resources.GetValueOrDefault(type, 0);
        }
        public ResourceData GetResourceData(ResourceType type)
        {
            return _resourceDatabase.GetValueOrDefault(type);
        }
        public Dictionary<ResourceType, int> GetAllResources()
        {
            return new Dictionary<ResourceType, int>(_resources);
        }
    }
}