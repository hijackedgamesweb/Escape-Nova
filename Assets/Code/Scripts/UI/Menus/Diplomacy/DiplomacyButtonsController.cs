using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Managers;
using Code.Scripts.UI.Menus.Trading;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.Diplomacy
{
    public class DiplomacyButtonsController : MonoBehaviour
    {
        [SerializeField] GameObject _tradingPanelPrefab;
        [SerializeField] Canvas _mainCanvas;
        private GameObject _tradePanelPrefab;
        Civilization _currentCivilization;
        public void SetCivilization(Civilization civ)
        {
            _currentCivilization = civ;
        }
        
        public void OnTradeButtonClicked()
        {
            if (_currentCivilization == null) return;
            _tradePanelPrefab = Instantiate(_tradingPanelPrefab, _mainCanvas.transform);
            TradeManager tradingPanel = _tradePanelPrefab.GetComponent<TradeManager>();
            tradingPanel.SetCivilization(_currentCivilization);
        }
        
        public void OnBlameClicked()
        {
            if (_currentCivilization == null) return;
            _currentCivilization.CivilizationState.ReceiveBlame();
        }
        
        public void OnBondClicked()
        {
            if (_currentCivilization == null) return;
            _currentCivilization.CivilizationState.ReceiveBond();
        }
        
    }
}