using System;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public class CivilizationScreen : BaseUIScreen
    {
        [SerializeField] private TMP_Text _civilizationNameText;
        [SerializeField] private TMP_Text _civilizationDescText;
        [SerializeField] private Image _civilizationIcon;
        [SerializeField] public Image _leaderSprite;
        [SerializeField] private Text _leaderNameText;
        [SerializeField] private Slider _hungerSlider;
        [SerializeField] private Slider _angerSlider;
        [SerializeField] private Slider _militaryPowerSlider;
        
        private Civilization _currentCivilization;
        private CivilizationManager _civilizationManager;
        
        private void Start()
        {
            ServiceLocator.GetService<CivilizationManager>();
        }

        public void SetCivilization(string civilizationName)
        {
            _civilizationManager = ServiceLocator.GetService<CivilizationManager>();
            _currentCivilization = _civilizationManager.GetCivilization(civilizationName);
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
            _hungerSlider.value = _currentCivilization.CivilizationState.FriendlinessLevel;
            _angerSlider.value = _currentCivilization.CivilizationState.DependencyLevel;
            _militaryPowerSlider.value = _currentCivilization.CivilizationState.InterestLevel;
        }
    }
}