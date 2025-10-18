using System;
using System.Collections.Generic;
using Code.Scripts.UI.Windows;
using UnityEngine;

namespace Code.Scripts.UI.Common
{
    public class UIRepository : MonoBehaviour
    {
        
        private readonly Dictionary<Type, BaseUIScreen> _uiPrefabs = new();

        private void Awake()
        {
            foreach (var ui in GetComponentsInChildren<BaseUIScreen>(true))
            {
                _uiPrefabs[ui.GetType()] = ui;
            }
        }
        

        public T GetUI<T>() where T : BaseUIScreen
        {
            if (_uiPrefabs.TryGetValue(typeof(T), out var uiPrefab))
            {
                return uiPrefab as T;
            }
            throw new Exception($"UI of type {typeof(T)} not found in repository.");
        }
    }
}