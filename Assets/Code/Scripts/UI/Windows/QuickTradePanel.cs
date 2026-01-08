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
        Quest,
        Message
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
        
        
        public void InitializeMessage(Civilization civilization, string message)
        {
            _currentCivilization = civilization;
            tradeType = QuickTradeType.Message;
            UpdatePanel();
            UpdateMessage(message);
            gameObject.SetActive(true);
        }
        
        private void UpdateMessage(string message)
        {
            _tradeText.text = message;
        }

        private void UpdatePanel()
        {
            _acceptButton.onClick.RemoveAllListeners();
            _declineButton.onClick.RemoveAllListeners();
            if (_currentCivilization == null) return;

            _civilizationIcon.sprite = _currentCivilization.CivilizationData.CivilizationIcon;

            _declineButton.gameObject.SetActive(true);
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
                case QuickTradeType.Message:
                    _tradeText.text =
                        $"The {_currentCivilization.CivilizationData.Name} civilization decided to leave your solar system due to some ethical disagreements. ";
                    _resourceAmountText.text = "";
                    _resourceIcon.color = Color.clear;
                    _declineButton.gameObject.SetActive(false);
                    break;
            }
            _acceptButton.onClick.AddListener(AcceptTrade);
            _declineButton.onClick.AddListener(DeclineTrade);
        }

        //TODO: Añadir cambios de estado en la civilizacion aqui
        private void DeclineTrade()
        {
            var state = _currentCivilization.CivilizationState;
            string civName = _currentCivilization.CivilizationData.Name;
            //rechazamos el tradeo
            switch (tradeType)
            {
                case QuickTradeType.Tribute:
                    Debug.Log("Tribute declined.");
                    //General: -10 confianza, -5 dependencia.
                    //Skulg: -5 confianza
                    //Mippip/Halxi: -5 dependencia
                    //Handoull/Akki: -5 interés
                    switch (civName)
                    {
                        case "Skulg":
                            state.TrustLevel -= 0.15f;
                            state.DependencyLevel -= 0.05f;
                            break;
                        case "Mippip":
                        case "Halxi":
                            state.TrustLevel -= 0.10f;
                            state.DependencyLevel -= 0.10f;
                            break;
                        case "Handoull":
                        case "Akki":
                            state.TrustLevel -= 0.10f;
                            state.DependencyLevel -= 0.05f;
                            state.InterestLevel -= 0.05f;
                            break;
                        default:
                            state.TrustLevel -= 0.10f;
                            state.DependencyLevel -= 0.05f;
                            break;
                    }
                    break;
                case QuickTradeType.Offrend:
                    Debug.Log("Offrend declined.");
                    //General: -10 Amistad
                    //Skulg: -10 Amistad
                    //Mippip/Halxi: -5 Confianza
                    //Handoull/Akki: -5 Interés
                    switch (civName)
                    {
                        case "Skulg":
                            state.FriendlinessLevel -= 0.20f;
                            break;
                        case "Mippip":
                        case "Halxi":
                            state.FriendlinessLevel -= 0.10f;
                            state.TrustLevel -= 0.05f;
                            break;
                        case "Handoull":
                        case "Akki":
                            state.FriendlinessLevel -= 0.10f;
                            state.InterestLevel -= 0.05f;
                            break;
                        default:
                            state.FriendlinessLevel -= 0.10f;
                            break;
                    }
                    break;
                case QuickTradeType.Quest:
                    Debug.Log("Quest declined.");
                    //General: -5 Amistad, -5 Dependencia
                    //Mippip: -5 Dependencia
                    //Akki: -5 Interes
                    //Halxi/Mippip: -5 Confianza
                    switch (civName)
                    {
                        case "Mippip":
                            state.FriendlinessLevel -= 0.05f;
                            state.DependencyLevel -= 0.10f;
                            state.TrustLevel -= 0.05f;
                            break;
                        case "Akki":
                            state.FriendlinessLevel -= 0.05f;
                            state.DependencyLevel -= 0.05f;
                            state.InterestLevel -= 0.05f;
                            break;
                        case "Halxi":
                            state.FriendlinessLevel -= 0.05f;
                            state.DependencyLevel -= 0.05f;
                            state.TrustLevel -= 0.05f;
                            break;
                        default:
                            state.FriendlinessLevel -= 0.05f;
                            state.DependencyLevel -= 0.05f;
                            break;
                    }
                    break;
                case QuickTradeType.Message:
                    Debug.Log($"Quest {_currentCivilization.CivilizationData.CivilizationQuest.Title} accepted.");
                    _questManager.StartSecondaryQuest(_currentCivilization.CivilizationData.CivilizationQuest);
                    break;
            }
            gameObject.SetActive(false);
        }

        private void AcceptTrade()
        {
            var state = _currentCivilization.CivilizationState;
            string civName = _currentCivilization.CivilizationData.Name;

            switch (tradeType)
            {
                // JUGADOR ENTREGA TRIBUTO
                case QuickTradeType.Tribute:
                    Debug.Log("Tribute accepted.");
                    // General: +10 Confianza, +5 Dependencia
                    // Extra Skulg: +5 Confianza
                    // Extra Mippip/Halxi: +5 Dependencia
                    // Extra Handoull/Akki: +5 Interés
                    switch (civName)
                    {
                        case "Skulg":
                            state.TrustLevel += 0.15f;
                            state.DependencyLevel += 0.05f;
                            break;
                        case "Mippip":
                        case "Halxi":
                            state.TrustLevel += 0.10f;
                            state.DependencyLevel += 0.10f;
                            break;
                        case "Handoull":
                        case "Akki":
                            state.TrustLevel += 0.10f;
                            state.DependencyLevel += 0.05f;
                            state.InterestLevel += 0.05f;
                            break;
                        default:
                            state.TrustLevel += 0.10f;
                            state.DependencyLevel += 0.05f;
                            break;
                    }
                    break;

                // JUGADOR ACEPTA OFRENDA
                case QuickTradeType.Offrend:
                    Debug.Log("Offrend accepted.");
                    // General: +10 Amistad
                    // Extra Mippip: +5 Amistad
                    // Extra Handoull: +10 Interés
                    // Extra Halxi/Akki: +5 Confianza
                    switch (civName)
                    {
                        case "Mippip":
                            state.FriendlinessLevel += 0.15f;
                            break;
                        case "Handoull":
                            state.FriendlinessLevel += 0.10f;
                            state.InterestLevel += 0.10f;
                            break;
                        case "Halxi":
                        case "Akki":
                            state.FriendlinessLevel += 0.10f;
                            state.TrustLevel += 0.05f;
                            break;
                        default:
                            state.FriendlinessLevel += 0.10f;
                            break;
                    }
                    break;

                // JUGADOR ACEPTA PETICIÓN (QUEST)
                case QuickTradeType.Quest:
                    Debug.Log($"Quest {_currentCivilization.CivilizationData.CivilizationQuest.Title} accepted.");
                    _questManager.StartSecondaryQuest(_currentCivilization.CivilizationData.CivilizationQuest);
                    //General: +5 Amistad, +5 Dependencia
                    //Mippip: +5 Dependencia
                    //Akki: +5 Interes
                    //Halxi/Mippip: +5 Confianza
                    switch (civName)
                    {
                        case "Mippip":
                            state.FriendlinessLevel += 0.05f;
                            state.DependencyLevel += 0.10f;
                            state.TrustLevel += 0.05f;
                            break;
                        case "Akki":
                            state.FriendlinessLevel += 0.05f;
                            state.DependencyLevel += 0.05f;
                            state.InterestLevel += 0.05f;
                            break;
                        case "Halxi":
                            state.FriendlinessLevel += 0.05f;
                            state.DependencyLevel += 0.05f;
                            state.TrustLevel += 0.05f;
                            break;
                        default:
                            state.FriendlinessLevel += 0.05f;
                            state.DependencyLevel += 0.05f;
                            break;
                    }
                    break;

                case QuickTradeType.Message:
                    break;
            }
            gameObject.SetActive(false);
        }

    }
}
