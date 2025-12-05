using System;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Menus.Diplomacy;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public class CivilizationScreen : BaseUIScreen
    {
        [SerializeField] private TMP_Text _civilizationNameText;
        [SerializeField] private TMP_Text _civilizationDescText;
        [SerializeField] private TMP_Text _currentMoodText;
        [SerializeField] private Image _civilizationIcon;
        [SerializeField] public Image _leaderSprite;
        [SerializeField] private Text _leaderNameText;
        [SerializeField] private Slider _friendshipSlider;
        [SerializeField] private Slider _dependencySlider;
        [SerializeField] private Slider _interestSlider;
        [SerializeField] private Slider _trustSlider;
        
        [SerializeField] private DiplomacyButtonsController _diplomacyButtonsPanel;
        
        private Civilization _currentCivilization;
        private CivilizationManager _civilizationManager;
        
        private void Start()
        {
            ServiceLocator.GetService<CivilizationManager>();
            UIEvents.OnUpdateCivilizationUI += UpdateUI;
        }
        
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
            }
        }

        public void SetCivilization(string civilizationName)
        {
            _civilizationManager = ServiceLocator.GetService<CivilizationManager>();
            _currentCivilization = _civilizationManager.GetCivilization(civilizationName);
            _diplomacyButtonsPanel.SetCivilization(_currentCivilization);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_currentCivilization == null) return;
            
            _civilizationNameText.text = _currentCivilization.CivilizationData.Name;
            _civilizationDescText.text = _currentCivilization.CivilizationData.CivilizationDescription;
           // _civilizationIcon.sprite = _currentCivilization.CivilizationData.CivilizationIcon;
            _leaderSprite.sprite = _currentCivilization.CivilizationData.LeaderPortrait;
           // _leaderNameText.text = _currentCivilization.LeaderName;
           _friendshipSlider.value = _currentCivilization.CivilizationState.FriendlinessLevel;
           _dependencySlider.value = _currentCivilization.CivilizationState.DependencyLevel;
           _interestSlider.value = _currentCivilization.CivilizationState.InterestLevel;
           _trustSlider.value = _currentCivilization.CivilizationState.TrustLevel;
           _currentMoodText.text = _currentCivilization.CivilizationState.GetMoodDescription();
        }
    }
}