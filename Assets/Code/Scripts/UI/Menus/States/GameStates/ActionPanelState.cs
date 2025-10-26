using System;
using System.Linq;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus.States.GameStates.InGameSubStates;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus.States.GameStates
{
    public class ActionPanelState : AState, IParametrizedState
    {
        private ActionPanelScreen _uiObject;
        private IStateManager _subStateManager;
        private Type _initialSubStateType;
        
        public ActionPanelState(IStateManager stateManager, BaseUIScreen uiObject, GameStateFactory _subStatesFactory) : base(stateManager)
        {
            _uiObject = (ActionPanelScreen) uiObject;
            _subStateManager = new StateManager(_subStatesFactory);
        }

        public override void Enter(IStateManager gameManager)
        {
            InitializeHUD();
            _uiObject.gameObject.SetActive(true);
            
            if(_initialSubStateType != null)
            {
                var method = typeof(IStateManager)
                    .GetMethods()
                    .FirstOrDefault(m => m.Name == "SetState" && m.IsGenericMethodDefinition);

                method?.Invoke(_subStateManager, null);
            }
            else
            {
                _subStateManager.SetState<DefaultState>();
            }
        }

        private void InitializeHUD()
        {
            _uiObject.astrariumBtn.onClick.AddListener(() => _subStateManager.SetState<AstrariumSubState>());
            //_uiObject.constructionBtn.onClick.AddListener(() => _subStateManager.SetState<ConstructionSubState>());
            _uiObject.storageBtn.onClick.AddListener(() => _subStateManager.SetState<StorageSubState>());
            _uiObject.diplomacyBtn.onClick.AddListener(() => _subStateManager.SetState<CivilizationSubState>());
            _uiObject.returnBtn.onClick.AddListener(() => _stateManager.SetState<InGameState>());
        }

        public override void Exit(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(false);
            _uiObject.returnBtn.onClick.RemoveAllListeners();
            _uiObject.storageBtn.onClick.RemoveAllListeners();
            //_uiObject.constructionBtn.onClick.RemoveAllListeners();
            _uiObject.astrariumBtn.onClick.RemoveAllListeners();
            _uiObject.diplomacyBtn.onClick.RemoveAllListeners();
        }

        public override void Update()
        {
        }


        public void SetParameter(object parameter)
        {
            if (parameter is Type stateType)
            {
                _initialSubStateType = stateType;
            }
        }
    }
}