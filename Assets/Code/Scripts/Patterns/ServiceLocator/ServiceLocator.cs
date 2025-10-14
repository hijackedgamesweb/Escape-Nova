using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Patterns.ServiceLocator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();
        public static void RegisterService<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is already registered. Overwriting.");
            }
            _services[type] = service;
        }
        public static T GetService<T>()
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            throw new Exception($"Service of type {type} is not registered.");
        }
        public static void UnregisterService<T>()
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
            else
            {
                Debug.LogWarning($"Service of type {type} is not registered.");
            }
        }
    }
}
