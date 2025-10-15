using System;
using UnityEngine;

public class EntityConstructionButton : MonoBehaviour
{
    //VARIABLES
    
    //AQUI HACE FALTA UNA REFERENCIA AL ADDRESSABLE DE UN SCRIPTABLE OBJECT

    [SerializeField] private string entityName;
    [SerializeField] private int timeToBuild;
    
    
    //METODOS

    private void Awake()
    {
        //EN EL AWAKE, CARGAMOS EL SCRIPTABLE OBJECT CORRESPONDIENTE A ESTA ENTIDAD Y ASIGNAMOS SU INFORMACION
    }
}
