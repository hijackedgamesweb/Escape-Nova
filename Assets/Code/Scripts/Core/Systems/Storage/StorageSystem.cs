using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;
using Mono.Cecil;
using ResourceType = Code.Scripts.Core.Systems.Resources.ResourceType;


namespace Code.Scripts.Core.Systems.Storage
{
    public class StorageSystem
    {
        private List<ResourceData> _resourceDataList; // Lista de recursos que asignamos en el Inspector
        private InventoryData _inventoryData;
        
        private Dictionary<ResourceType, ResourceData> _resourceDatabase; // Diccionario para buscar datos de recursos rápido
        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>(); // Aquí guardamos cuánto tenemos de cada recurso
        private Dictionary<string, int> _inventoryItems = new Dictionary<string, int>(); // Para items de inventario
        
        // Eventos para avisar cuando cambian los recursos
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
            // Me registro en el ServiceLocator para que otros sistemas me encuentren
            ServiceLocator.RegisterService<StorageSystem>(this);
            // Inicializo la base de datos de recursos
            InitializeResourceDatabase();
            InitializeInventoryItems();
        }
        
        public void InitializeInventoryItems()
        {
            _inventoryItems.Clear();

            if (_inventoryData == null || _inventoryData.items == null)
            {
                Debug.LogWarning("No se proporcionó un InventoryData al StorageSystem.");
                return;
            }

            foreach (var item in _inventoryData.items)
            {
                if (item.itemData != null && !string.IsNullOrEmpty(item.itemData.itemName))
                {
                    // ¡Clave! Añadimos el item al diccionario con su CANTIDAD INICIAL
                    _inventoryItems[item.itemData.itemName] = item.quantity;
                    Debug.Log($"Item de inventario registrado: {item.itemData.itemName} (Cantidad: {item.quantity})");
                }
            }
            Debug.Log($"Inventario inicializado con {_inventoryItems.Count} items.");
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
                Debug.LogWarning($"Item {itemName} no encontrado en inventario disponible");
                return false;
            }

            _inventoryItems[itemName] += quantity;
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

        
        private void InitializeResourceDatabase()
        {
            _resourceDatabase = new Dictionary<ResourceType, ResourceData>();
            
            // Recorro la lista de recursos que asignamos en el Inspector
            if (_resourceDataList != null && _resourceDataList.Count > 0)
            {
                foreach (var resource in _resourceDataList)
                {
                    if (resource != null)
                    {
                        // Guardo cada recurso en el diccionario para acceso rápido
                        _resourceDatabase[resource.Type] = resource;
                        // Inicializo la cantidad en 0
                        _resources[resource.Type] = 0;
                        Debug.Log($"Registered resource: {resource.Type} - {resource.DisplayName}");
                    }
                }
            }
            else
            {
                Debug.LogError("No se han asignado recursos en el Sistema de Almacenamiento!");
            }
            
            Debug.Log($"Base de datos de recursos inicializada con {_resourceDatabase.Count} recursos");
        }
        
        // Método para AGREGAR recursos
        public bool AddResource(ResourceType type, int amount)
        {
            // Verifico que el recurso exista en la base de datos
            if (!_resourceDatabase.ContainsKey(type))
            {
                Debug.LogError($"Tipo de recurso {type} no encontrado en la base de datos");
                return false;
            }
            
            // Si es la primera vez que tenemos este recurso, lo inicializo en 0
            if (!_resources.ContainsKey(type))
                _resources[type] = 0;
                
            // Obtengo el máximo permitido para este recurso
            var maxStack = _resourceDatabase[type].MaxStack;
            var newAmount = _resources[type] + amount;
            
            // Verifico si me paso del límite máximo
            if (newAmount > maxStack)
            {
                _resources[type] = maxStack;
                Debug.LogWarning($"Recurso de {type} ha llegado al máximo de {maxStack}!");
            }
            else
            {
                _resources[type] = newAmount;
            }
            
            // Aviso a todos los que estén escuchando que este recurso cambió
            OnResourceChanged?.Invoke(type, _resources[type]);
            OnStorageUpdated?.Invoke();
            return true;
        }
        
        // Método para CONSUMIR recursos
        public bool ConsumeResource(ResourceType type, int amount)
        {
            // Verifico que tengamos suficientes recursos
            if (!HasResource(type, amount))
                return false;
                
            // Resto la cantidad consumida
            _resources[type] -= amount;
            // Aviso que el recurso cambió
            OnResourceChanged?.Invoke(type, _resources[type]);
            OnStorageUpdated?.Invoke();
            return true;
        }
        
        // Verifico si tengo cierta cantidad de un recurso
        public bool HasResource(ResourceType type, int amount)
        {
            return _resources.ContainsKey(type) && _resources[type] >= amount;
        }
        
        // Obtengo la cantidad actual de un recurso
        public int GetResourceAmount(ResourceType type)
        {
            return _resources.GetValueOrDefault(type, 0);
        }
        
        // Obtengo los datos de configuración de un recurso
        public ResourceData GetResourceData(ResourceType type)
        {
            return _resourceDatabase.GetValueOrDefault(type);
        }
        
        // Obtengo una copia de todos los recursos que tenemos
        public Dictionary<ResourceType, int> GetAllResources()
        {
            return new Dictionary<ResourceType, int>(_resources);
        }
    }
}