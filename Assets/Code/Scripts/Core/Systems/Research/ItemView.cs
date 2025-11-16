using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemView : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private Image itemPreviewImage;
    [SerializeField] private GameObject selectionHighlight;

    private ItemData itemData;
    private System.Action<ItemView> onSelectCallback;

    public void Initialize(ItemData data, System.Action<ItemView> onSelect)
    {
        itemData = data;
        onSelectCallback = onSelect;
        UpdateUI();
        SetSelected(false);
    }

    public void SetAmount(int amount)
    {
        if (itemAmountText != null)
            itemAmountText.text = amount.ToString();
    }
    
    private void UpdateUI()
    {
        if (itemData != null)
        {
            if (itemNameText != null)
                itemNameText.text = itemData.displayName;
            
            if (itemPreviewImage != null)
            {
                if (itemData.icon != null)
                {
                    itemPreviewImage.sprite = itemData.icon;
                    itemPreviewImage.color = Color.white;
                }
                else
                {
                    itemPreviewImage.sprite = null;
                    itemPreviewImage.color = new Color(1,1,1,0); 
                }
            }
        }
    }

    public bool IsSelected => selectionHighlight != null && selectionHighlight.activeSelf;
    
    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(selected);
    }

    public ItemData GetItemData() => itemData;

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelectCallback?.Invoke(this);
    }
}