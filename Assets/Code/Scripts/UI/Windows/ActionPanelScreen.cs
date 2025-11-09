using System;
using Code.Scripts.Core.Managers;
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
        [SerializeField] public Button storageBtn;
        [SerializeField] public Button missionsBtn;
        [SerializeField] public Button researchBtn;
        
        [SerializeField] public BaseUIScreen astrariumPanel;
        [SerializeField] public BaseUIScreen diplomacyPanel;
        [SerializeField] public BaseUIScreen skillTreePanel;
        [SerializeField] public BaseUIScreen constructionPanel;
        [SerializeField] public BaseUIScreen storagePanel;
        [SerializeField] public BaseUIScreen missionsPanel;
        [SerializeField] public BaseUIScreen researchPanel;

        private BaseUIScreen _currentPanel;
        public void Awake()
        {
            astrariumBtn.onClick.AddListener(() => 
                Show("Astrarium"));
            diplomacyBtn.onClick.AddListener(() => 
                Show("Diplomacy"));
            skillTreeBtn.onClick.AddListener(() => 
                Show("SkillTree"));
          //  constructionBtn.onClick.AddListener(() => 
          //      Show("Construction"));
            storageBtn.onClick.AddListener(() => 
                Show("Storage"));
            missionsBtn.onClick.AddListener(() => 
                Show("Missions"));
            researchBtn.onClick.AddListener(() => 
                Show("Research"));
            returnBtn.onClick.AddListener(() => UIManager.Instance.ShowScreen<InGameScreen>());
            
        }


        public override void Show(object parameter = null)
        {
            base.Show(parameter);
            if (_currentPanel != null)
            {
                _currentPanel.Hide();
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
                case "SkillTree":
                    skillTreePanel.Show();
                    _currentPanel = skillTreePanel;
                    break;
               // case "Construction":
                    constructionPanel.Show();
                    _currentPanel = constructionPanel;
                    break;
                case "Storage":
                    storagePanel.Show();
                    _currentPanel = storagePanel;
                    break;
                case "Missions":
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