using UnityEngine;
using UnityEngine.UI;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Windows 
{
    public class StorageCrafting_TabManager : BaseUIScreen
    {
        [Header("Pestañas (Botones)")]
        [SerializeField] private Button storageTabButton;
        [SerializeField] private Button craftingTabButton;

        [Header("Paneles (Hijos)")]
        [SerializeField] private GameObject storagePanel;
        [SerializeField] private GameObject craftingPanel;

        [Header("Estilo de Pestañas")]
        [SerializeField] private Color tabNormalColor = Color.gray;
        [SerializeField] private Color tabSelectedColor = Color.white;

        private void Awake()
        {
            if (storageTabButton != null)
                storageTabButton.onClick.AddListener(ShowStoragePanel);
            
            if (craftingTabButton != null)
                craftingTabButton.onClick.AddListener(ShowCraftingPanel);
        }

        public override void Show(object parameter = null)
        {
            base.Show(parameter);
            
            ShowStoragePanel();
        }

        public void ShowStoragePanel()
        {
            if (storagePanel != null) storagePanel.SetActive(true);
            if (craftingPanel != null) craftingPanel.SetActive(false);

            if (storageTabButton != null)
                storageTabButton.GetComponent<Image>().color = tabSelectedColor;
            if (craftingTabButton != null)
                craftingTabButton.GetComponent<Image>().color = tabNormalColor;
        }
        
        public void ShowCraftingPanel()
        {
            if (storagePanel != null) storagePanel.SetActive(false);
            if (craftingPanel != null) craftingPanel.SetActive(true);

            if (storageTabButton != null)
                storageTabButton.GetComponent<Image>().color = tabNormalColor;
            if (craftingTabButton != null)
                craftingTabButton.GetComponent<Image>().color = tabSelectedColor;
        }
    }
}