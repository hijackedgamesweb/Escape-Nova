using Code.Scripts.Core.Events;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus.States.GameStates;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class GameStateManager : MonoBehaviour, IStateManager
    {
        private IState _currentState;

        void Start()
        {
            // Opcional: Establece un estado inicial, por ejemplo, "Jugando"
            // _currentState = new PlayingState(this); 
            // _currentState.Enter(this);
        }
        private void OnEnable()
        {
            SystemEvents.OnGameOver += HandleGameOver;
        }
        private void OnDisable()
        {
            SystemEvents.OnGameOver -= HandleGameOver;
        }
        private void HandleGameOver()
        {
            Debug.Log("GameStateManager ha recibido OnGameOver. Cambiando a GameOverState...");
            SetState(new GameOverState(this));
        }
        public void SetState(IState state)
        {
            _currentState?.Exit(this);
            _currentState = state;
            _currentState.Enter(this);
        }

        public IState GetCurrentState()
        {
            return _currentState;
        }
        void Update()
        {
            _currentState?.Update();
        }
    }
}