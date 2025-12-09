using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Events;
using Code.Scripts.UI.Menus.Trading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Managers
{
    public class TradeManager : MonoBehaviour
    {
        private Entity.Entity _playerEntity;
        private Civilization _targetEntity;
        
        [SerializeField] private TradingPanel _playerTradingPanel;
        [SerializeField] private TradingPanel _targetTradingPanel;
        
        [SerializeField] private Image _leaderPortrait;
        [SerializeField] private TMP_Text _civilizationName;
        [SerializeField] private Image _background;
        
        public void InitializeTrade(Entity.Entity playerEntity, Civilization targetEntity)
        {
            _playerEntity = playerEntity;
            _targetEntity = targetEntity;
            _leaderPortrait.sprite = targetEntity.CivilizationData.LeaderPortrait;
            _civilizationName.text = targetEntity.CivilizationData.Name;
            _background.sprite = targetEntity.CivilizationData.CivilizationFlag;
            _playerTradingPanel.InitializePanel(_playerEntity.StorageSystem);
            _targetTradingPanel.InitializePanel(_targetEntity.StorageSystem);
        }

        public void OfferTrade()
        {
            TradingData playerTradeData = _playerTradingPanel.GetTradingData();
            TradingData targetTradeData = _targetTradingPanel.GetTradingData();
            
            // Implement trade logic here
            int playerOfferValue = TradeOfferCalculator.CalculateTotalOfferValue(playerTradeData, _playerEntity);
            int targetOfferValue = TradeOfferCalculator.CalculateTotalOfferValue(targetTradeData, _targetEntity);
            
            float friendshipModifier = 0.5f + (_targetEntity.CivilizationState.FriendlinessLevel);
            
            if (playerOfferValue * friendshipModifier >= targetOfferValue)
            {
                // Execute trade
                _playerEntity.StorageSystem.ExecuteTrade(playerTradeData, targetTradeData, _targetEntity.StorageSystem);
                _targetEntity.AcceptTradeOffer();
                CloseTradePanel();
                DiplomacyEvents.OnTradeProposed?.Invoke((Civilization)_targetEntity, true);
            }
            else
            {
                _targetEntity.DenyTradeOffer();
                DiplomacyEvents.OnTradeProposed?.Invoke((Civilization)_targetEntity, false);
            }
        }
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        public void SetCivilization(Civilization civ)
        {
            _targetEntity = civ;
        }
        
        public void CloseTradePanel()
        {
            Destroy(gameObject);
        }
    }
}
