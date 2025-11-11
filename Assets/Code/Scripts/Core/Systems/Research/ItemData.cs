using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string displayName;
    public Sprite icon;
    public int maxStack = 1;
    [TextArea] public string description;
    [Header("Desbloqueo")]
    public ConstructibleDataSO itemToUnlock;
}