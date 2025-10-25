using UnityEngine;

[CreateAssetMenu(fileName = "NewAttribute", menuName = "Create/Attribute")]
public class AttributeSO : ScriptableObject
{
    public string attributeName;
    public string description;
    public Sprite thumbnail;
}
