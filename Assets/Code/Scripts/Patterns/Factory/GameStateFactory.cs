using System;
using System.Collections.Generic;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.Patterns.State.States.GameStates;
using Code.Scripts.UI.Menus.States.GameStates;
using Code.Scripts.UI.Windows;
using UnityEngine;

namespace Code.Scripts.Patterns.Factory
{
    public class GameStateFactory
    {
        private readonly Dictionary<Type, BaseUIScreen> _uiPrefabs = new();
        
        public GameStateFactory()
        {
        }
        
        public void RegisterUI<T>(BaseUIScreen uiPrefab) where T : IState
        {
            _uiPrefabs[typeof(T)] = uiPrefab;
        }
        
        public T Create<T>(IStateManager stateManager) where T : AState
        {
            if (typeof(T) == typeof(MainMenuState))
                return (T)(AState)new MainMenuState(stateManager, _uiPrefabs[typeof(MainMenuState)]);
            if (typeof(T) == typeof(InGameState))
                return (T)(AState)new InGameState(stateManager, _uiPrefabs[typeof(InGameState)]);
            if (typeof(T) == typeof(OptionsState))
                return (T)(AState)new OptionsState(stateManager, _uiPrefabs[typeof(OptionsState)]);

            throw new Exception($"No factory for type {typeof(T)}");
        }
        
    }
}