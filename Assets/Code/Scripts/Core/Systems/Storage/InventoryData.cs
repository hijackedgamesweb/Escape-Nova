using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Storage
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]
    public class InventoryData : ScriptableObject
    {
        public List<InventoryItem> items;
    }
}