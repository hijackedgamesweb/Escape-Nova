using UnityEngine;
using UnityEngine.UI;

public class ConstructionUI : MonoBehaviour
{
    //VARIABLES
    [SerializeField] private GameObject placingUI;
    
    [SerializeField] private GameObject PlanetsTab;
    [SerializeField] private GameObject SatelitesTab;
    [SerializeField] private Button buildBtn;
    
    [SerializeField] private GameObject planetsLayoutGroup;
    [SerializeField] private GameObject satelitesLayoutGroup;
    
    //METODOS

    public void TabPressed(int idx)
    {
        switch (idx)
        {
            case 0:
                planetsLayoutGroup.SetActive(true);
                satelitesLayoutGroup.SetActive(false);
                break;
            case 1:
                satelitesLayoutGroup.SetActive(true);
                planetsLayoutGroup.SetActive(false);
                break;
        }
    }
    
    
    public void EntityPressed()
    {
        buildBtn.interactable = true;
    }
    
    
    public void BuildButtonPressed()
    {
        placingUI.SetActive(true);
        gameObject.SetActive(false);
    }
}