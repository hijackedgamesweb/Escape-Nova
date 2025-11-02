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
        itemAmountText.text = amount.ToString();
    }
    
    private void UpdateUI()
    {
        if (itemData != null)
        {
            itemNameText.text = itemData.itemName;
            
            if (itemPreviewImage != null && itemData.icon != null)
            {
                itemPreviewImage.sprite = itemData.icon;
                itemPreviewImage.color = Color.white;
            }
            else
            {
                if (itemPreviewImage != null)
                {
                    itemPreviewImage.sprite = null;
                    itemPreviewImage.color = new Color(0,0,0,0);
                }
            }
        }
        else
        {
            Debug.LogWarning("Se intentó actualizar la UI sin ItemData asignado en: " + gameObject.name);
            itemNameText.text = "Error Item";
            if (itemPreviewImage != null)
            {
                itemPreviewImage.sprite = null;
                itemPreviewImage.color = new Color(0,0,0,0);
            }
        }
    }

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