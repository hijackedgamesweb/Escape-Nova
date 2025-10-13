using UnityEngine;

[CreateAssetMenu(fileName = "PlanetScriptableObject", menuName = "Scriptable Objects/PlanetScriptableObject")]
public class PlanetScriptableObject : ScriptableObject
{
    //ENUMS
    public enum PlanetType
    {
        Rocky,
        Metalic,
        Icy,
        Volcanic,
        Desertic
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
    public PlanetType[] planetType;
    public Resources[] generatedResources;
    public int[] resourcesGeneratedAmount;
    
    public int tier;
    public int size;
    public int timeToBuild;


    //METODOS
}