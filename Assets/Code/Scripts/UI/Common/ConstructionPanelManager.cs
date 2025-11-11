using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Common
{
    public enum ConstructionType
    {
        Planet,
        Satelite
    }
    public class ConstructionPanelManager : MonoBehaviour
    {
        [SerializeField] private GameObject planetContent;
        [SerializeField] private GameObject sateliteContent;
        
        [SerializeField] private GameObject planetButton;
        [SerializeField] private GameObject sateliteButton;
        
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;
        
        public static Action<ConstructionType> OnConstructionTypeChanged;
    
        public void ShowPlanetContent()
        {
            planetContent.SetActive(true);
            sateliteContent.SetActive(false);
            planetButton.GetComponent<Image>().color = activeColor;
            sateliteButton.GetComponent<Image>().color = inactiveColor;
            OnConstructionTypeChanged?.Invoke(ConstructionType.Planet);
        }
    
        public void ShowSateliteContent()
        {
            planetContent.SetActive(false);
            sateliteContent.SetActive(true);
            planetButton.GetComponent<Image>().color = inactiveColor;
            sateliteButton.GetComponent<Image>().color = activeColor;
            OnConstructionTypeChanged?.Invoke(ConstructionType.Satelite);
        }
    
    }
}
