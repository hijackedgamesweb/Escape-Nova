using System;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Necesario para la lista de botones

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
        private Dictionary<string, Button> _buttonLookup;

        public void Awake()
        {
            _buttonLookup = new Dictionary<string, Button>
            {
                { "Astrarium", astrariumBtn },
                { "Diplomacy", diplomacyBtn },
                { "Constellations", skillTreeBtn },
                { "Storage", storageCraftingBtn },
                { "Objectives", missionsBtn },
                { "Research", researchBtn },
            };


            astrariumBtn.onClick.AddListener(() => Show("Astrarium"));
            diplomacyBtn.onClick.AddListener(() => Show("Diplomacy"));
            skillTreeBtn.onClick.AddListener(() => Show("Constellations"));
            storageCraftingBtn.onClick.AddListener(() => Show("Storage"));
            missionsBtn.onClick.AddListener(() => Show("Objectives"));
            researchBtn.onClick.AddListener(() => Show("Research"));
            returnBtn.onClick.AddListener(() => UIManager.Instance.ShowScreen<InGameScreen>());
            
            SystemEvents.OnInventoryUnlocked += EnableStorageButton;
            SystemEvents.OnResearchUnlocked += EnableResearchButton;
            SystemEvents.OnConstellationsUnlocked += EnableConstellationsButton;
            
            if (SystemEvents.IsInventoryUnlocked)
            {
                EnableStorageButton();
            }
            else
            {
                storageCraftingBtn.interactable = false;
            }

            if (SystemEvents.IsResearchUnlocked)
            {
                EnableResearchButton();
            }
            else
            {
                researchBtn.interactable = false;
            }
            
            if (SystemEvents.IsConstellationsUnlocked)
            {
                EnableConstellationsButton();
            }
            else
            {
                skillTreeBtn.interactable = false;
            }
        }

        private void OnDestroy()
        {
            SystemEvents.OnInventoryUnlocked -= EnableStorageButton;
            SystemEvents.OnResearchUnlocked -= EnableResearchButton;
            SystemEvents.OnConstellationsUnlocked -= EnableConstellationsButton;
        }

        private void EnableStorageButton()
        {
            storageCraftingBtn.interactable = true;
        }

        private void EnableResearchButton()
        {
            researchBtn.interactable = true;
        }
        
        private void EnableConstellationsButton()
        {
            skillTreeBtn.interactable = true;
        }
        
        private void ResetButtonHighlights()
        {
            foreach (var button in _buttonLookup.Values)
            {
                if (button != null)
                {
                    button.interactable = button.IsInteractable(); 
                    button.targetGraphic.color = button.colors.normalColor;
                }
            }
        }

        public override void Show(object parameter = null)
        {
            base.Show(parameter);

            if (_currentPanel != null)
            {
                _currentPanel.Hide();
            }
            string panelName = parameter as string;
            ResetButtonHighlights(); 
            
            if (!string.IsNullOrEmpty(panelName) && _buttonLookup.TryGetValue(panelName, out Button selectedButton))
            {
                if (selectedButton != null)
                {
                    selectedButton.targetGraphic.color = selectedButton.colors.selectedColor;
                }
            }
            
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