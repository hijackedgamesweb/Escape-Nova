using UnityEngine;

[System.Serializable]
public class PlayerAttributes
{
    public AttributeSO attribute;
    public int amount;

    public PlayerAttributes(AttributeSO attribute, int amount)
    {
        this.attribute = attribute;
        this.amount = amount;
    }
}
