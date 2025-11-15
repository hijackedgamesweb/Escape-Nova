using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.Scripts.Utilities;

public class IngredientSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI detailsText;
    
    [SerializeField] private Color hasColor = Color.green;
    [SerializeField] private Color missingColor = Color.red;

    public void SetData(Sprite sprite, string itemName, int amountOwned, int amountNeeded)
    {
        string formattedOwned = NumberFormatter.FormatNumber(amountOwned, 1);
        string formattedNeeded = NumberFormatter.FormatNumber(amountNeeded, 1);

        if (icon != null)
        {
            icon.sprite = sprite;
            icon.color = (sprite != null) ? Color.white : new Color(1,1,1,0);
        }
        
        if (detailsText != null)
        {
            detailsText.text = $"{formattedOwned}/{formattedNeeded} {itemName}\n";
        
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