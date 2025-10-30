using System;
using System.Collections.Generic;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Systems.Civilization;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class CivilizationManager : MonoBehaviour
    {
        List<Civilization> _civilizations = new();
        public List<Civilization> GetCivilizations => _civilizations;
        
        // EVENTS
        public Action<Civilization> OnNewCivilizationDiscovered;
        private void Awake()
        {
            ServiceLocator.RegisterService(this);
        }

        public void AddCivilization(Civilization civ)
        {
            _civilizations.Add(civ);
            OnNewCivilizationDiscovered?.Invoke(civ);
        }
        
        public void UpdateCivilizations(WorldContext context)
        {
            foreach (var civ in _civilizations)
            {
                civ.AIController.UpdateAI(context);
            }
        }
        
        public void UpdateCivilizations(WorldContext context, ICommand command)
        {
            foreach (var civ in _civilizations)
            {
                civ.AIController.UpdateAI(context, command);
            }
        }

        public Civilization GetCivilization(string civilizationName)
        {
            if (_civilizations.Exists(civ => civ.CivilizationData.Name == civilizationName))
            {
                return _civilizations.Find(civ => civ.CivilizationData.Name == civilizationName);
            }
            else
            {
                Debug.Log("Civilization not found");
                return null;
            }
        }
        
    }
}