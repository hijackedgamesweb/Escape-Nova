using UnityEngine;
using UnityEngine.UI;

public class ConstructionCanvas : MonoBehaviour
{
    //VARIABLES
    
    [SerializeField] private GameObject PlanetsTab;
    [SerializeField] private GameObject SatelitesTab;
    [SerializeField] private Button buildBtn;
    
    
    //METODOS
    
    public void EntityPressed()
    {
        buildBtn.interactable = true;
    }
}