using System.Collections.Generic;
using UnityEngine;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

public class InventoryTester : MonoBehaviour
{
    [SerializeField] private List<ItemData> availableItems; // Asigna tus ScriptableObjects aquí en el Inspector
    
    private void Start()
    {
        InitializeAndTestInventory();
    }
    
    private void InitializeAndTestInventory()
    {
        // 1. Primero necesitas crear el StorageSystem (si no existe)
        StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
        
        if (storage == null)
        {
            Debug.LogError("StorageSystem no encontrado. Debe ser creado primero.");
            return;
        }
        
        // 2. Inicializar los items disponibles
        storage.InitializeInventoryItems(availableItems);
        
        // 3. Agregar algunos items de prueba
        storage.AddInventoryItem("Espada", 1);
        storage.AddInventoryItem("Papiro", 5);
        storage.AddInventoryItem("Sol", 3);
        
        // 4. Obtener y mostrar los items
        var itemQuantities = storage.GetInventoryItems();
        
        Debug.Log("=== INVENTARIO ACTUAL ===");
        foreach (var item in itemQuantities)
        {
            Debug.Log($"Item: {item.Key} - Cantidad: {item.Value}");
        }
        
        // 5. Probar otras funciones
        Debug.Log($"¿Tengo Espada? {storage.HasInventoryItem("Espada", 1)}");
        Debug.Log($"Cantidad de Papiro: {storage.GetInventoryItemQuantity("Papiro")}");
        
        // 6. Probar consumir items
        if (storage.ConsumeInventoryItem("Sol", 1))
        {
            Debug.Log("Sol consumida exitosamente");
            Debug.Log($"Soles restantes: {storage.GetInventoryItemQuantity("Sol")}");
        }
    }
}