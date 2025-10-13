using UnityEngine;

[CreateAssetMenu(fileName = "AstrariumEntryScriptableObject", menuName = "Scriptable Objects/AstrariumEntryScriptableObjectScript")]
public class AstrariumEntryScriptableObjectScript : ScriptableObject
{
    //ENUMS
    public enum EntryType
    {
        Planet,
        Resource,
        Satelite,
        Species,
        Other
    }
    
    
    //VARIABLES
    public EntryType entryType;
    public string name;
    public string description;
    public Sprite image;
    
    
    //METODOS
    
}