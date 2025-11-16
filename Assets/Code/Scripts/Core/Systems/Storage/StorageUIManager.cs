using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems.Storage;
using System.Linq; // Necesario para la búsqueda por clave

namespace Code.Scripts.Core.Systems.Storage
{
    public class StorageUIManager : MonoBehaviour
    {
        [Header("Prefab and Container")]
        [SerializeField] private GameObject itemViewPrefab;
        [SerializeField] private Transform itemsContainer;
        
        [Header("Description Panel")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI descriptionItemName;
        [SerializeField] private Image descriptionItemSprite;
        
        private List<ItemView> itemViews = new List<ItemView>();
        private ItemView selectedItemView;
        private StorageSystem _storageSystem;
    
        private void Start()
        {
            try
            {
                _storageSystem = ServiceLocator.GetService<StorageSystem>();
                _storageSystem.OnStorageUpdated += RefreshStorageUI; 
            }
            catch (System.Exception e)
            {
                return;
            }
            
            RefreshStorageUI();
            UpdateDescriptionPanel(null);
        }
    
        private void OnDestroy()
        {
            if (_storageSystem != null)
            {
                _storageSystem.OnStorageUpdated -= RefreshStorageUI;
            }
        }
        
        public void RefreshStorageUI()
        {
            if (_storageSystem == null) return;
    
            Dictionary<string, int> itemQuantities = _storageSystem.GetInventoryItems();
            ItemData previouslySelectedItemData = selectedItemView?.GetItemData();
            Dictionary<string, ItemView> existingViews = itemViews.ToDictionary(view => view.GetItemData().itemName, view => view);
            itemViews.Clear();
            
            bool selectionRestored = false;
            foreach (KeyValuePair<string, int> itemEntry in itemQuantities)
            {
                string itemName = itemEntry.Key;
                int amount = itemEntry.Value;
                
                if (amount > 0)
                {
                    ItemData data = _storageSystem.GetItemData(itemName); 
                    
                    if (data != null)
                    {
                        if (existingViews.TryGetValue(itemName, out ItemView view))
                        {
                            view.SetAmount(amount);
                            itemViews.Add(view);
                            existingViews.Remove(itemName);
                        }
                        else
                        {
                            GameObject newItemGO = Instantiate(itemViewPrefab, itemsContainer);
                            ItemView newView = newItemGO.GetComponent<ItemView>();
                            if (newView != null)
                            {
                                newView.Initialize(data, OnItemSelected);
                                newView.SetAmount(amount);
                                itemViews.Add(newView);
                                view = newView;
                            }
                        }
                        if (previouslySelectedItemData != null && previouslySelectedItemData.itemName == itemName)
                        {
                            selectedItemView = view;
                            selectedItemView.SetSelected(true);
                            UpdateDescriptionPanel(data);
                            selectionRestored = true;
                        }
                    }
                }
            }
            foreach (var viewToDestroy in existingViews.Values)
            {
                Destroy(viewToDestroy.gameObject);
            }
            if (!selectionRestored)
            {
                selectedItemView = null;
                UpdateDescriptionPanel(null);
            }
        }
        
        private void OnItemSelected(ItemView selectedView)
        {
            if (selectedItemView == selectedView)
            {
                selectedItemView.SetSelected(false);
                selectedItemView = null;
                UpdateDescriptionPanel(null); 
            }
            else
            {
                if (selectedItemView != null)
                {
                    selectedItemView.SetSelected(false);
                }
                selectedItemView = selectedView;
                selectedItemView.SetSelected(true);
                UpdateDescriptionPanel(selectedView.GetItemData());
            }
        }
    
        private void UpdateDescriptionPanel(ItemData data)
        {
            if (data == null)
            {
                descriptionItemName.text = "---";
                descriptionText.text = "Select an inventory item to see the details.";
                descriptionItemSprite.sprite = null;
                descriptionItemSprite.color = new Color(1,1,1,0); 
                return;
            }
            
            descriptionItemName.text = data.displayName;
            descriptionText.text = data.description;
            
            if (descriptionItemSprite != null)
            {
                if (data.icon != null)
                {
                    descriptionItemSprite.sprite = data.icon;
                    descriptionItemSprite.color = Color.white;
                }
                else
                {
                    descriptionItemSprite.sprite = null;
                    descriptionItemSprite.color = new Color(1,1,1,0);
                }
            }
        }
    }
}