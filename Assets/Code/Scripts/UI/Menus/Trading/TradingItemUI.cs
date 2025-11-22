using Code.Scripts.Core.Systems.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.Trading
{
    public class TradingItemUI : MonoBehaviour
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text itemAmountText;
        private InventoryItem _item;
        public void SetItemData(InventoryItem item)
        {
            _item = item;
            itemIcon.sprite = _item.itemData.icon;
            itemAmountText.text = "0";
        }

        public void AddItem()
        {
            int currentAmount = int.Parse(itemAmountText.text);
            if(currentAmount >= _item.quantity) return;
            currentAmount++;
            itemAmountText.text = currentAmount.ToString();
        }
        
        public void RemoveItem()
        {
            int currentAmount = int.Parse(itemAmountText.text);
            if (currentAmount <= 0) return;
            currentAmount--;
            itemAmountText.text = currentAmount.ToString();
        }

        public InventoryItem GetItemData()
        {
            int selectedAmount = int.Parse(itemAmountText.text);
            return new InventoryItem
            {
                itemData = _item.itemData,
                quantity = selectedAmount
            };
        }
    }
}