using System.Collections.Generic;
using Code.Scripts.Core.Systems.Storage;
using UnityEngine;

namespace Code.Scripts.UI.Menus.Trading
{
    public class TradingObjectsGrid : MonoBehaviour
    {
        [SerializeField] private GameObject itemPrefab;
        private List<TradingItemUI> _tradingItemUIs = new List<TradingItemUI>();

        public void InitializeGrid(List<InventoryItem> inventoryData)
        {
            foreach (InventoryItem item in inventoryData)
            {
                if(item.quantity <= 0) continue;
                GameObject itemObject = Instantiate(itemPrefab, transform);
                TradingItemUI itemUI = itemObject.GetComponent<TradingItemUI>();
                itemUI.SetItemData(item);
                _tradingItemUIs.Add(itemUI);
            }
        }
        
        public List<InventoryItem> GetInventoryItems()
        {
            List<InventoryItem> items = new List<InventoryItem>();
            foreach (TradingItemUI itemUI in _tradingItemUIs)
            {
                items.Add(itemUI.GetItemData());
            }
            return items;
        }
        
    }
}