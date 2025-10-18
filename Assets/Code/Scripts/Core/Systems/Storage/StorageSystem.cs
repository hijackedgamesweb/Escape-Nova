using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;


namespace Code.Scripts.Core.Systems.Storage
{
    public class StorageSystem : MonoBehaviour
    {
        [SerializeField] private List<ResourceData> _resourceDataList; // Lista de recursos que asignamos en el Inspector
        
        private Dictionary<ResourceType, ResourceData> _resourceDatabase; // Diccionario para buscar datos de recursos rápido
        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>(); // Aquí guardamos cuánto tenemos de cada recurso
        
        // Eventos para avisar cuando cambian los recursos
        public event Action<ResourceType, int> OnResourceChanged;
        public event Action OnStorageUpdated;
        
        private void Awake()
        {
            // Me registro en el ServiceLocator para que otros sistemas me encuentren
            ServiceLocator.RegisterService<StorageSystem>(this);
            // Inicializo la base de datos de recursos
            InitializeResourceDatabase();
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