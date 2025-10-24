using UnityEngine;

[CreateAssetMenu(fileName = "SateliteSO", menuName = "Scriptable Objects/SateliteSO")]
public class SateliteSO : EntityScriptableObjectScript
{
    //ENUMS
    public enum SateliteType
    {
        Science,
        Construction
    }
    
    
    //VARIABLES
    public SateliteType type;
    
    public int timeReduction; //La cantidad de ciclos solares que reduce para la construccion o investigacion
    

    //METODOS
}
