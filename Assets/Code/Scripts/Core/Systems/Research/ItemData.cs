using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string displayName;
    public Sprite icon;
    public int maxStack = 1;
    [TextArea] public string description;
    [Header("Desbloqueo")]
    public ConstructibleDataSO itemToUnlock;
}