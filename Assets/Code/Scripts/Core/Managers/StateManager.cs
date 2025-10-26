using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.State.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Core.Managers
{
    public class StateManager :  IStateManager
    {
        private IState _currentState;

        public StateManager()
        {
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
    }
}