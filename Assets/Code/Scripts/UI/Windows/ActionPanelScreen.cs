using System;
using Code.Scripts.Core.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public class ActionPanelScreen : BaseUIScreen
    {
        [SerializeField] public Button returnBtn;
        [SerializeField] public Button astrariumBtn;
        [SerializeField] public Button diplomacyBtn;
        [SerializeField] public Button skillTreeBtn;
        [SerializeField] public Button constructionBtn;
        [SerializeField] public Button storageCraftingBtn;
        [SerializeField] public Button missionsBtn;
        [SerializeField] public Button researchBtn;

        [SerializeField] public TextMeshProUGUI panelTitleText;
        
        [SerializeField] public BaseUIScreen astrariumPanel;
        [SerializeField] public BaseUIScreen diplomacyPanel;
        [SerializeField] public BaseUIScreen skillTreePanel;
        [SerializeField] public BaseUIScreen constructionPanel;
        [SerializeField] public BaseUIScreen storageCraftingPanel;
        [SerializeField] public BaseUIScreen missionsPanel;
        [SerializeField] public BaseUIScreen researchPanel;

        private BaseUIScreen _currentPanel;

        public void Awake()
        {
            astrariumBtn.onClick.AddListener(() => Show("Astrarium"));
            diplomacyBtn.onClick.AddListener(() => Show("Diplomacy"));
            skillTreeBtn.onClick.AddListener(() => Show("Constellations"));
            storageCraftingBtn.onClick.AddListener(() => Show("Storage"));
            missionsBtn.onClick.AddListener(() => Show("Objectives"));
            researchBtn.onClick.AddListener(() => Show("Research"));
            returnBtn.onClick.AddListener(() => UIManager.Instance.ShowScreen<InGameScreen>());
        }

        public override void Show(object parameter = null)
        {
            base.Show(parameter);
            
            AudioManager.Instance.PlaySFX("ButtonClick");
            
            if (_currentPanel != null)
            {
                _currentPanel.Hide();
            }
            
            string panelName = parameter as string;
            if (panelTitleText != null)
            {
                panelTitleText.text = !string.IsNullOrEmpty(panelName) ? panelName : "Panel"; 
            }

            switch (parameter)
            {
                case "Astrarium":
                    astrariumPanel.Show();
                    _currentPanel = astrariumPanel;
                    break;
                case "Diplomacy":
                    diplomacyPanel.Show();
                    _currentPanel = diplomacyPanel;
                    break;
                case "Constellations":
                    skillTreePanel.Show();
                    _currentPanel = skillTreePanel;
                    break;
                case "Construction":
                    constructionPanel.Show();
                    _currentPanel = constructionPanel;
                    break;
                case "Storage":
                    storageCraftingPanel.Show();
                    _currentPanel = storageCraftingPanel;
                    break;
                case "Objectives":
                    missionsPanel.Show();
                    _currentPanel = missionsPanel;
                    break;
                case "Research":
                    researchPanel.Show();
                    _currentPanel = researchPanel;
                    break;
            }
        }
    }
}