using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Core.Systems.Storage;

public class StorageUIManager : MonoBehaviour
{
    [Header("Prefab and Container")]
    [SerializeField] private GameObject itemViewPrefab;
    [SerializeField] private Transform itemsContainer;
    
    [Header("Item Database")]
    [SerializeField] private InventoryData inventoryDataSource;

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
        itemQuantities = new Dictionary<string, int>();

        if (inventoryDataSource == null)
        {
            Debug.LogError("No se ha asignado un 'Inventory Data Source' al StorageUIManager!");
            return;
        }

        foreach (InventoryItem item in inventoryDataSource.items)
        {
            if (item.itemData != null && !itemQuantities.ContainsKey(item.itemData.itemName))
            {
                itemQuantities.Add(item.itemData.itemName, item.quantity);
            }
        }
    } 
    
    private void InitializeStorageUI()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }

        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
        itemViews.Clear();

        if (inventoryDataSource == null) return;

        foreach (InventoryItem item in inventoryDataSource.items)
        {
            if (item.itemData != null)
            {
                GameObject newItemGO = Instantiate(itemViewPrefab, itemsContainer);
                ItemView view = newItemGO.GetComponent<ItemView>();

                if (view != null)
                {
                    view.Initialize(item.itemData, OnItemSelected);
                    view.SetAmount(item.quantity);
                    itemViews.Add(view);
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