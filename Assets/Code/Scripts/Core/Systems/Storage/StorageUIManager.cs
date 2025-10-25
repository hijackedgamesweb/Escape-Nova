using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    [Header("Item Data")]
    [SerializeField] private List<ItemData> items;

    private List<ItemView> itemViews = new List<ItemView>();
    private ItemView selectedItemView;

    private void Start()
    {
        InitializeStorageUI();
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
                    itemViews.Add(itemView);
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