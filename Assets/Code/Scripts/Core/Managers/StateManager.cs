using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.Patterns.State.States.GameStates;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Core.Managers
{
    public class StateManager :  IStateManager
    {
        private readonly GameStateFactory _factory;
        private IState _currentState;

        public StateManager(GameStateFactory factory)
        {
            _factory = factory;
        }

        public void StartGame()
        {
            throw new System.NotImplementedException();
        }

        public void EndGame()
        {
            throw new System.NotImplementedException();
        }

        public IState GetCurrentState()
        {
            return _currentState;
        }

        public void SetState(IState state)
        {
            _currentState?.Exit(this);
            _currentState = state;
            _currentState.Enter(this);
        }

        // Método práctico para crear y cambiar estado usando la Factory
        public void SetState<T>() where T : AState
        {
            IState newState = _factory.Create<T>(this);
            SetState(newState);
        }
    }
}