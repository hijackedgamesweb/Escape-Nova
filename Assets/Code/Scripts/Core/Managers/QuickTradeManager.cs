using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Patterns.Singleton;
using Code.Scripts.UI.Windows;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class QuickTradeManager : Singleton<QuickTradeManager>
    {
        
        [SerializeField] QuickTradePanel _currentQuickTradePanel;
        
        public void CreateTributeOffer(Civilization civilization)
        {
            _currentQuickTradePanel.InitializeTribute(civilization); 
        }
        
        public void CreateGiftOffer(Civilization civilization)
        {
            _currentQuickTradePanel.InitializeOffrend(civilization); 
        }

        public void CreateQuestOffer(Civilization civilization)
        {
            _currentQuickTradePanel.InitializeQuest(civilization);
        }
        
        public void CreateMessage(Civilization civilization, string message)
        {
            _currentQuickTradePanel.InitializeMessage(civilization, message);
        }

    }
}