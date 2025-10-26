using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.State.Interfaces;
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

        public void SetState<T>(object parameter = null) where T : AState
        {
            IState newState = _factory.Create<T>(this);
            if (newState is IParametrizedState parametrizedState)
            {
                parametrizedState.SetParameter(parameter);
            }
            SetState(newState);
        }
    }
}