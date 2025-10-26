using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

public class StorageUIManager : MonoBehaviour
{
    [Header("Prefab and Container")]
    [SerializeField] private GameObject itemViewPrefab;
    [SerializeField] private Transform itemsContainer;

    [Header("Description Panel")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI descriptionItemName;
    [SerializeField] private Image descriptionItemSprite;
    
    private List<ItemView> itemViews = new List<ItemView>();
    private ItemView selectedItemView;
    
    private Dictionary<string, int> itemQuantities;

    private void Start()
    {
        InitializeItemQuantities();
        InitializeStorageUI();
    }

    private void InitializeItemQuantities()
    {
        StorageSystem storage = ServiceLocator.GetService<StorageSystem>();

        storage.AddInventoryItem("Espada", 1);
        storage.AddInventoryItem("Papiro", 5);
        
        if (storage != null)
        {
            // Obtener solo los items del inventario (no recursos)
            itemQuantities = storage.GetInventoryItems();
            
            Debug.Log($"Inventario inicializado con {itemQuantities.Count} items");
        }
        else
        {
            Debug.LogError("StorageSystem no disponible");
            itemQuantities = new Dictionary<string, int>();
        }
    }
    
    private void InitializeStorageUI()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }

        InitializeExistingItems();
    }

    private void InitializeExistingItems()
    {
        itemViews.Clear();
        
        foreach (Transform child in itemsContainer)
        {
            ItemView itemView = child.GetComponent<ItemView>();
            if (itemView != null)
            {
                ItemData data = itemView.GetItemData();
                if (data != null)
                {
                    itemView.Initialize(OnItemSelected);
                    
                    if (itemQuantities != null && itemQuantities.ContainsKey(data.itemName))
                    {
                        itemView.SetAmount(itemQuantities[data.itemName]);
                    }
                    else
                    {
                        itemView.SetAmount(0);
                    }
                    itemViews.Add(itemView);
                }
            }
        }
    }
    
    public void RefreshQuantities(Dictionary<string, int> newQuantities)
    {
        itemQuantities = newQuantities;
        
        foreach (ItemView itemView in itemViews)
        {
            ItemData data = itemView.GetItemData();
            if (itemQuantities.ContainsKey(data.itemName))
            {
                itemView.SetAmount(itemQuantities[data.itemName]);
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
        if (descriptionPanel == null) return;

        if (descriptionItemName != null)
            descriptionItemName.text = data.itemName;

        if (descriptionText != null)
            descriptionText.text = data.description;

        if (descriptionItemSprite != null && data.icon != null)
        {
            descriptionItemSprite.sprite = data.icon;
            descriptionItemSprite.color = Color.white;
        }

        descriptionPanel.SetActive(true);
    }
}