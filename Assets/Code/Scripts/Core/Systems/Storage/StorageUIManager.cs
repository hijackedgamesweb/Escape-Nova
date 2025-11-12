using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems.Storage;

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
    
            foreach (Transform child in itemsContainer)
            {
                Destroy(child.gameObject);
            }
            itemViews.Clear();
            
            if (selectedItemView != null)
            {
                selectedItemView = null;
            }
    
            foreach (KeyValuePair<string, int> itemEntry in itemQuantities)
            {
                if (itemEntry.Value > 0) 
                {
                    ItemData data = _storageSystem.GetItemData(itemEntry.Key); 
                    
                    if (data != null)
                    {
                        GameObject newItemGO = Instantiate(itemViewPrefab, itemsContainer);
                        ItemView view = newItemGO.GetComponent<ItemView>();
    
                        if (view != null)
                        {
                            view.Initialize(data, OnItemSelected);
                            view.SetAmount(itemEntry.Value);
                            itemViews.Add(view);
                        }
                    }
                }
            }
        }
        private void OnItemSelected(ItemView selectedView)
        {
            if (selectedItemView != null)
            {
                selectedItemView.SetSelected(false);
            }
            
            selectedItemView = selectedView;
            selectedItemView.SetSelected(true);
    
            UpdateDescriptionPanel(selectedView.GetItemData());
        }
    
        private void UpdateDescriptionPanel(ItemData data)
        {
            if ( data == null) return;
    
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