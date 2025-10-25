using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int amount;
    public Sprite icon;
    [TextArea(3, 10)]
    public string description;
}