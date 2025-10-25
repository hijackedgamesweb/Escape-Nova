using System;
using UnityEngine;
using UnityEngine.UI;

public class PlacingUI : MonoBehaviour
{
    //VARIABLES
    
    [SerializeField] private GameObject ConstructionUI;

    public string entityToBuildName; //El nombre de la entidad que se va a construir
    
    //METODOS
    
    public void PlacingLocationPressed()
    {
        
        //Cambiamos de interfaz
        ConstructionUI.SetActive(true); //Activamos la interfaz de construccion
        gameObject.SetActive(false); //Desactivamos esta interfaz
    }
}