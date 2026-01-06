using System;
using System.Collections.Generic;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Civilization;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.Command.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class CivilizationManager : MonoBehaviour, ISaveable
    {
        List<Civilization> _civilizations = new();
        public List<Civilization> GetCivilizations => _civilizations;
        
        // EVENTS
        public Action<Civilization, bool> OnNewCivilizationDiscovered;
        private void Awake()
        {
            ServiceLocator.RegisterService(this);
        }

        public void AddCivilization(Civilization civ)
        {
            _civilizations.Add(civ);
            OnNewCivilizationDiscovered?.Invoke(civ, true);
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
        
        public Civilization GetFirstCivilization()
        {
            if (_civilizations.Count > 0)
            {
                return _civilizations[0];
            }
            else
            {
                Debug.Log("No civilizations available");
                return null;
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
        
        public void TryAssignPlanetToCivilization(Planet newPlanet)
        {
            if (newPlanet == null || newPlanet.PlanetData == null) return;

            foreach (var civ in _civilizations)
            {
                if (civ.HasHomePlanetAssigned) continue;
                if (civ.CivilizationData.HomePlanetData == newPlanet.PlanetData)
                {
                    Debug.Log($"[CivilizationManager] Planeta '{newPlanet.Name}' asignado a {civ.CivilizationData.Name}");
                    civ.AssignHomePlanet();
                    newPlanet.EstablishContact(civ);
                    if (civ.AIController is BaseBehaviour aiBehaviour)
                    {
                        if (aiBehaviour._isAtWarWithPlayer)
                        {
                            Debug.Log($"[CivilizationManager] {civ.CivilizationData.Name} está en guerra. El planeta entra en conflicto.");
                            newPlanet.DeclareWar(civ);
                            aiBehaviour.StartWar(); 
                        }
                    }
                    return;
                }
            }
        }

        public string GetSaveId()
        {
            return "CivilizationManager";
        }

        public JToken CaptureState()
        {
            JObject obj = new JObject();
            JArray civsArray = new JArray();
            foreach (var civ in _civilizations)
            {
                civsArray.Add(civ.CaptureState());
            }
            obj["civilizations"] = civsArray;
            return obj;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = (JObject)state;
            JArray civsArray = (JArray)obj["civilizations"];
            _civilizations.Clear();
            foreach (var civToken in civsArray)
            {
                var civName = civToken["CivilizationData"]["Name"].ToObject<string>();
                var civDataSO = Resources.Load<CivilizationSO>($"Civilizations/{civName}");
                Civilization civ = new Civilization();
                civ.SetCivilizationData(civDataSO);
                civ.RestoreState(civToken);
                OnNewCivilizationDiscovered?.Invoke(civ, false);
                _civilizations.Add(civ);
            }
        }

        public void RemoveCivilization(Civilization civilization)
        {
            if (_civilizations.Contains(civilization))
            {
                _civilizations.Remove(civilization);
                OnNewCivilizationDiscovered?.Invoke(civilization, false);
            }
        }
    }
}