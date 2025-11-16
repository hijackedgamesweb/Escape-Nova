using Code.Scripts.Core.Managers; // Importante para tener acceso al UIManager
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using UnityEngine;

namespace Code.Scripts.UI.Menus.States.GameStates
{
    public class GameOverState : IState
    {
        private readonly IStateManager _stateManager;
        
        public GameOverState(IStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        public void Enter(IStateManager gameManager)
        {
            UIManager uiManager = ServiceLocator.GetService<UIManager>();

            if (uiManager == null)
            {
                return;
            }

            uiManager.ShowGameOverScreen();
        }
        public void Update()
        {
        }
        public void Exit(IStateManager gameManager)
        {
            UIManager uiManager = ServiceLocator.GetService<UIManager>();
            uiManager.HideGameOverScreen();
        }
    }
}