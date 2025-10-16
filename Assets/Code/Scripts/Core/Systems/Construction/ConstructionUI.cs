using System;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionUI : MonoBehaviour
{
    //VARIABLES
    [SerializeField] private GameObject placingUI;
    
    [SerializeField] private GameObject PlanetsTab;
    [SerializeField] private GameObject SatelitesTab;
    private int currentTab = 0; //0: Planets / 1: Satelites
    
    [SerializeField] private Button buildBtn;
    [SerializeField] private GameObject errorMsg;
    
    [SerializeField] private GameObject planetsLayoutGroup;
    [SerializeField] private GameObject satelitesLayoutGroup;
    
    [SerializeField] private GameObject entitieConstructionButtonPrefab;
    
    //METODOS
    private void Awake()
    {
        //PARA LA SIMULACION DE LA ALFA, CREAMOS NOSOTROS MISMOS DOS ENTIDADES, EN EL JUGO FINAL NO SERA ASI
        
        //CREACION DE UNA ENTRADA DE PLANETA
        GameObject plnt = Instantiate(entitieConstructionButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity); //Creamos el boton
        plnt.transform.SetParent(planetsLayoutGroup.transform, false); //Añadimos el boton como hijo de "plaentsLayoutGroup"
        
        plnt.GetComponent<EntityConstructionButton>().entityName = "Earth"; //Añadimos a la variable del boton el nombre de la entidad que construye
        plnt.GetComponent<EntityConstructionButton>().entityType = "Planet"; //Añadimos a la variable del boton el tipo de la entidad que construye
        
        //CREACION DE UNA ENTRADA DE SATELITE
        GameObject stlt = Instantiate(entitieConstructionButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity); //Creamos el boton
        stlt.transform.SetParent(satelitesLayoutGroup.transform, false); //Añadimos el boton como hijo de "SatelitesLayoutGroup"
        
        stlt.GetComponent<EntityConstructionButton>().entityName = "Ares"; //Añadimos a la variable del boton el nombre de la entidad que construye
        stlt.GetComponent<EntityConstructionButton>().entityType = "Satelite"; //Añadimos a la variable del boton el tipo de la entidad que construye
    }


    public void TabPressed(int idx)
    {
        switch (idx)
        {
            case 0:
                planetsLayoutGroup.SetActive(true);
                satelitesLayoutGroup.SetActive(false);
                currentTab = 0; //Current Tab -> Planets
                break;
            case 1:
                planetsLayoutGroup.SetActive(false);
                satelitesLayoutGroup.SetActive(true);
                currentTab = 1; //Current Tab -> Satelites
                break;
        }
    }
    
    
    public void EntityPressed()
    {
        buildBtn.interactable = true;
    }
    
    
    public void BuildButtonPressed()
    {
        if (CheckForReources())
        {
            placingUI.SetActive(true);
            gameObject.SetActive(false);
            if(errorMsg.activeSelf) { errorMsg.SetActive(false); }
        }
        else { errorMsg.SetActive(true); }
    }
    
    
    private bool CheckForReources()
    {
        //AQUI COMPROBAR SI EL JUGADOR TIENE SUFICIENTES RECURSOS PARA CONSTRUIR EL ELEMENTO SELECCIONADO
        return true;
    }
}