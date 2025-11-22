using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.UI.Menus.Trading;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class TradeManager : MonoBehaviour
    {
        private Entity.Entity _playerEntity;
        private Entity.Entity _targetEntity;
        
        [SerializeField] private TradingPanel _playerTradingPanel;
        [SerializeField] private TradingPanel _targetTradingPanel;
        
        public void InitializeTrade(Entity.Entity playerEntity, Entity.Entity targetEntity)
        {
            _playerEntity = playerEntity;
            _targetEntity = targetEntity;
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
            
            if (playerOfferValue >= targetOfferValue)
            {
                // Execute trade
                Debug.Log("Trade Accepted");
            }
            else
            {
                Debug.Log("Trade Rejected");
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
    }
}
