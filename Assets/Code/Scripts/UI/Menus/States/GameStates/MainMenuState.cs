using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.State.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.Scripts.Patterns.State.States.GameStates
{
    public class MainMenuState : AState
    {

        private readonly GameObject _uiObject;
        
        public MainMenuState(IStateManager stateManager, GameObject uiObject) : base(stateManager)
        {
            _uiObject = uiObject;
        }

        public override void Enter(IStateManager gameManager)
        {
            _uiObject.SetActive(true);

            var playBtn = _uiObject.GetComponentInChildren<Button>();
            if (playBtn != null)
            {
                playBtn.onClick.AddListener(OnPlayClicked);
            }
        }


        public override void Exit(IStateManager gameManager)
        {
            _uiObject.SetActive(false);

            var playBtn = _uiObject.GetComponentInChildren<Button>();
            if (playBtn != null)
            {
                playBtn.onClick.RemoveListener(OnPlayClicked);
            }
        }

        public override void Update()
        {
        }
        
        
        private void OnPlayClicked()
        {
            _stateManager.SetState<InGameState>();
        }
    }
}