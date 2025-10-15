using UnityEngine;

[CreateAssetMenu(fileName = "EntityScriptableObjectScript", menuName = "Scriptable Objects/EntityScriptableObjectScript")]
public class EntityScriptableObjectScript : ScriptableObject
{
    //ENUMS
    public enum EntityType
    {
        Planet,
        Satelite
    }
    
    public enum Resources
    {
        Stone,
        Metal,
        Ice,
        Fire,
        Sand
    }
    
    
    //VARIABLES
    public EntityType entityType;

    public string entityName;
    public int tier;
    public int size;
    public int timeToBuild;
    
    public Resources[] buildResources;
    public int[] buildCost;
    
    
    //METODOS
}
