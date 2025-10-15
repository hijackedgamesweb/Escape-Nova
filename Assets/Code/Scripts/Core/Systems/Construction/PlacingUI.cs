using System;
using UnityEngine;
using UnityEngine.UI;

public class PlacingUI : MonoBehaviour
{
    //VARIABLES
    
    [SerializeField] private GameObject ConstructionUI;
    
    //METODOS
    
    public void PlacingLocationPressed()
    {
        Debug.Log("PLANETA:" + " X " + "CONSTRUIDO");
        ConstructionUI.SetActive(true); //Activamos la interfaz de construccion
        gameObject.SetActive(false); //Desactivamos esta interfaz
    }
}