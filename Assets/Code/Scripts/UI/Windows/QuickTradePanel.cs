using System;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public enum QuickTradeType
    {
        Offrend,
        Tribute,
        Quest
    }
    
    public class QuickTradePanel : MonoBehaviour
    {
        [SerializeField] private Image _civilizationIcon;
        [SerializeField] private TMP_Text _tradeText;
        [SerializeField] private TMP_Text _resourceAmountText;
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _declineButton;

        private Civilization _currentCivilization;

        private QuickTradeType tradeType;
        private QuestManager _questManager;

        private void Start()
        {
            _questManager = ServiceLocator.GetService<QuestManager>();
        }

        public void InitializeOffrend(Civilization civilization)
        {
            _currentCivilization = civilization;
            tradeType = QuickTradeType.Offrend;
            UpdatePanel();
            gameObject.SetActive(true);
        }

        public void InitializeTribute(Civilization civilization)
        {
            _currentCivilization = civilization;
            tradeType = QuickTradeType.Tribute;
            UpdatePanel();
            gameObject.SetActive(true);
        }
        
        
        public void InitializeQuest(Civilization civilization)
        {
            _currentCivilization = civilization;
            tradeType = QuickTradeType.Quest;
            UpdatePanel();
            gameObject.SetActive(true);
        }

        private void UpdatePanel()
        {
            _acceptButton.onClick.RemoveAllListeners();
            _declineButton.onClick.RemoveAllListeners();
            if (_currentCivilization == null) return;

            _civilizationIcon.sprite = _currentCivilization.CivilizationData.CivilizationIcon;

            switch (tradeType)
            {
                case QuickTradeType.Tribute:
                    _tradeText.text =
                        $"The {_currentCivilization.CivilizationData.Name} is demanding one {_currentCivilization.CivilizationData.TributeItem.displayName} as a tribute!";
                    _resourceAmountText.text = $"x{_currentCivilization.CivilizationData.TributeAmount}";
                    _resourceIcon.sprite = _currentCivilization.CivilizationData.TributeItem.icon;
                    _resourceAmountText.alignment = TextAlignmentOptions.Right;
                    _resourceIcon.color = Color.white;
                    break;
                case QuickTradeType.Offrend:
                    _tradeText.text =
                        $"The {_currentCivilization.CivilizationData.Name} is offering one {_currentCivilization.CivilizationData.OffrendItem.displayName} as a gift!";
                    _resourceAmountText.text = $"x{_currentCivilization.CivilizationData.OffrendAmount}";
                    _resourceAmountText.alignment = TextAlignmentOptions.Right;
                    _resourceIcon.sprite = _currentCivilization.CivilizationData.OffrendItem.icon;
                    _resourceIcon.color = Color.white;
                    break;
                case QuickTradeType.Quest:
                    _tradeText.text =
                        $"The {_currentCivilization.CivilizationData.Name} have a quest for you: {_currentCivilization.CivilizationData.CivilizationQuest.Description}";
                    _resourceAmountText.text = "Reward: " + _currentCivilization.CivilizationData.CivilizationQuest.Rewards[0].Description;
                    _resourceAmountText.alignment = TextAlignmentOptions.Center;
                    _resourceIcon.color = Color.clear;
                    break;
            }
            _acceptButton.onClick.AddListener(AcceptTrade);
            _declineButton.onClick.AddListener(DeclineTrade);
        }

        //TODO: Añadir cambios de estado en la civilizacion aqui
        private void DeclineTrade()
        {
            switch (tradeType)
            {
                case QuickTradeType.Tribute:
                    Debug.Log("Tribute declined.");
                    break;
                case QuickTradeType.Offrend:
                    Debug.Log("Offrend declined.");
                    break;
                case QuickTradeType.Quest:
                    Debug.Log("Quest declined.");
                    break;
            }
            gameObject.SetActive(false);
        }

        private void AcceptTrade()
        {
            switch (tradeType)
            {
                case QuickTradeType.Tribute:
                    Debug.Log("Tribute accepted.");
                    break;
                case QuickTradeType.Offrend:
                    Debug.Log("Offrend accepted.");
                    break;
                case QuickTradeType.Quest:
                    Debug.Log($"Quest {_currentCivilization.CivilizationData.CivilizationQuest.Title} accepted.");
                    _questManager.StartSecondaryQuest(_currentCivilization.CivilizationData.CivilizationQuest);
                    break;
            }
            gameObject.SetActive(false);
        }

    }
}
