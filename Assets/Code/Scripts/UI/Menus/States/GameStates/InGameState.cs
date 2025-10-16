using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.State.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Patterns.State.States.GameStates
{
    public class InGameState : AState
    {
        private GameObject _uiObject;
        public InGameState(IStateManager stateManager, GameObject uiObject) : base(stateManager)
        {
            _uiObject = uiObject;
        }

        public override void Enter(IStateManager gameManager)
        {
            Debug.Log("Entered InGame");
            _uiObject.SetActive(true);
        }

        public override void Exit(IStateManager gameManager)
        {
            Debug.Log("Exiting InGame");
            
            _uiObject.SetActive(false);
        }

        public override void Update()
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                _stateManager.SetState<MainMenuState>();
            }
        }
    }
}