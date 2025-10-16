using UnityEngine;

[CreateAssetMenu(fileName = "PlanetSO", menuName = "Scriptable Objects/PlanetSO")]
public class PlanetSO : EntityScriptableObjectScript
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
    
    
    //VARIABLES
    public PlanetType[] planetType;
    
    public Resources[] generatedResources;
    public int[] resourcesGeneratedAmount;
    

    //METODOS
}
