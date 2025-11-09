using System;
using UnityEngine;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_AddItemToInventory : AbstractResearchReward
    {
        public string itemName;
        public int amount = 1;
        public override void ApplyReward()
        {
            StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
            if (storage == null)
            {
                Debug.LogError("StorageSystem no encontrado.");
                return;
            }
            if (storage.AddInventoryItem(itemName, amount))
            {
                Debug.Log($"Recompensa: Añadido {amount}x {itemName} al inventario.");
            }
            else
            {
                Debug.LogWarning($"Recompensa: Se intentó añadir {itemName} pero el item no existe.");
            }
        } 
        public override string GetDescription() => $"Añadir {amount} x {itemName} al inventario";
    }
}