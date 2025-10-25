using System;
using System.Collections.Generic;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus.States.GameStates;
using Code.Scripts.UI.Windows;
using UnityEngine;

namespace Code.Scripts.Patterns.Factory
{
    public class GameStateFactory
    {
        private readonly Dictionary<Type, BaseUIScreen> _uiPrefabs = new();
        private readonly Dictionary<Type, Func<IStateManager, IState>> _constructors = new();
        
        public GameStateFactory()
        {
        }
        
        
        public void RegisterUI<T, TUI>(TUI ui, Func<IStateManager, TUI, T> constructor) where T : IState where TUI : BaseUIScreen
        {
            _uiPrefabs[typeof(T)] = ui;
            _constructors[typeof(T)] = (manager) => constructor(manager, ui);
        }
        
        public void Register<TState>(Func<IStateManager, TState> constructor)
            where TState : AState
        {
            _constructors[typeof(TState)] = (manager) => constructor(manager);
        }
        
        public T Create<T>(IStateManager stateManager) where T : AState
        {
            return (T)_constructors[typeof(T)].Invoke(stateManager);
        }
        
    }
}