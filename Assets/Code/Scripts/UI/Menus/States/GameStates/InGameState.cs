using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus.States.GameStates;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Patterns.State.States.GameStates
{
    public class InGameState : AState
    {
        private InGameScreen _uiObject;
        public InGameState(IStateManager stateManager, BaseUIScreen uiObject) : base(stateManager)
        {
            _uiObject = (InGameScreen) uiObject;
        }

        public override void Enter(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(true);
            _uiObject.returnBtn.onClick.AddListener(OnReturnButtonClicked);
        }

        public override void Exit(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(false);
            _uiObject.returnBtn.onClick.RemoveListener(OnReturnButtonClicked);
        }

        public override void Update()
        {
        }
        
        private void OnReturnButtonClicked()
        {
            _stateManager.SetState<MainMenuState>();
        }
    }
}