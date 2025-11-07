using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI detailsText;
    
    [SerializeField] private Color hasColor = Color.green;
    [SerializeField] private Color missingColor = Color.red;

    public void SetData(Sprite sprite, string itemName, int amountOwned, int amountNeeded)
    {
        Debug.Log($"--- IngredientSlotUI.SetData() --- \nItem: {itemName}, Owned: {amountOwned}, Needed: {amountNeeded}, Sprite? {sprite != null}", this.gameObject);

        if (icon != null)
        {
            icon.sprite = sprite;
            icon.color = (sprite != null) ? Color.white : new Color(1,1,1,0);
        }
        
        if (detailsText != null)
        {
            detailsText.text = $"{amountOwned}/{amountNeeded} {itemName}";
        
            if (amountOwned >= amountNeeded)
            {
                detailsText.color = hasColor;
            }
            else
            {
                detailsText.color = missingColor;
            }
        }
    }
}