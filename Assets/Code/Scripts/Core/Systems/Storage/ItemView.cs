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

    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    private System.Action<ItemView> onSelectCallback;
    private void Start()
    {
        UpdateUI();
    }

    public void Initialize(System.Action<ItemView> onSelect)
    {
        onSelectCallback = onSelect;
        UpdateUI();
        SetSelected(false);
    }

    private void UpdateUI()
    {
        if (itemData != null)
        {
            itemNameText.text = itemData.itemName;
            itemAmountText.text = itemData.amount.ToString();
            
            if (itemPreviewImage != null && itemData.icon != null)
            {
                itemPreviewImage.sprite = itemData.icon;
                itemPreviewImage.color = Color.white;
            }
        }
        else
        {
            Debug.LogWarning("No hay ItemData asignado en: " + gameObject.name);
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