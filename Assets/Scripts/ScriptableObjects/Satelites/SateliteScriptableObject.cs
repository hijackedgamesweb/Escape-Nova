using UnityEngine;

[CreateAssetMenu(fileName = "SateliteScriptableObject", menuName = "Scriptable Objects/SateliteScriptableObject")]
public class SateliteScriptableObject : ScriptableObject
{
    //ENUMS
    public enum SateliteType
    {
        Science,
        Construction
    }
    
    
    //VARIABLES
    public SateliteType type;
    public int tier;
    public int timeReduction; //La cantidad de ciclos solares que reduce para la construccion o investigacion



    //METODOS
    
}