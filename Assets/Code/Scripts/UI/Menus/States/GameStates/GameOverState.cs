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
            Debug.Log("GAME OVER - Mostrando pantalla...");
        }
        public void Update()
        {
        }
        public void Exit(IStateManager gameManager)
        {
        }
    }
}