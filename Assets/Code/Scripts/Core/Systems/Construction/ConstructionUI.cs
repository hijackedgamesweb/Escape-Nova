using UnityEngine;
using UnityEngine.UI;

public class ConstructionUI : MonoBehaviour
{
    //VARIABLES
    [SerializeField] private GameObject placingUI;
    
    [SerializeField] private GameObject PlanetsTab;
    [SerializeField] private GameObject SatelitesTab;

    [SerializeField] private Button buildBtn;

    [SerializeField] private GameObject errorMsg;
    
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