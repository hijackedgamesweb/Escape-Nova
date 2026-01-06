using Code.Scripts.Core.Events;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Civilization.AI;
using Code.Scripts.Core.Systems.Diplomacy.AI;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.Command;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Scripts.Core.Entity.Civilization
{
    public class Civilization : Entity, ISaveable
    {
        public CivilizationData CivilizationData {get; private set; }
        public CivilizationState CivilizationState  {get; private set; }
        private CommandInvoker _invoker;
        public IAIController AIController { get; private set; }
        public AIType AIControllerType;
        
        public Civilization(CommandInvoker invoker, CivilizationSO civilizationSO) : base(invoker, civilizationSO, null)
        {
            _invoker = invoker;
            
            SetCivilizationData(civilizationSO);
        }

        public Civilization() : base()
        {
            CivilizationData = new CivilizationData();
            CivilizationState = new CivilizationState();
            StorageSystem = new StorageSystem();
        }
        
        public void SetCivilizationData(CivilizationSO civilizationSO)
        {
            CivilizationData = new CivilizationData(civilizationSO);
            CivilizationState = new CivilizationState(civilizationSO);
            ItemPreferences = new EntityItemPreferences(civilizationSO.itemPreferences);
            StorageSystem = new StorageSystem(civilizationSO.startingResources, civilizationSO.startingInventory);
            StorageSystem.SetResourceAmounts(civilizationSO.startingResources, civilizationSO.startingResourceAmounts);
            AIController = AIControllerFactory.CreateAIController(civilizationSO.aiController, this, _invoker);
            AIControllerType = civilizationSO.aiController;
            ConstructionEvents.OnPlanetAdded += OnPlanetAdded;
            ConstructionEvents.OnConstructibleCreated += OnConstructibleCreated;
            ResearchEvents.OnResearchCompleted += OnResearchCompleted;
            DiplomacyEvents.OnCivilizationDiscovered += OnCivilizationDiscovered;
            MissionEvents.OnMissionCompleted += OnMissionCompleted;
        }

        public void AcceptTradeOffer()
        {
            CivilizationState.AddDependency(0.1f);
            CivilizationState.AddFriendliness(0.05f);
            CivilizationState.AddTrust(0.05f);
            switch (CivilizationData.Name)
            {
                case "Mippip":
                    CivilizationState.AddFriendliness(0.05f);
                    CivilizationState.AddTrust(0.05f);
                    CivilizationState.AddInterest(0.05f);
                    break;
                case "Handoull":
                    CivilizationState.AddInterest(0.05f);
                    break;
                    
                case "Akki":
                    CivilizationState.AddInterest(0.05f);
                    break;
            }
            
        }

        public void DenyTradeOffer()
        {
            CivilizationState.AddDependency(-0.1f);
            CivilizationState.AddFriendliness(-0.05f);
            CivilizationState.AddTrust(-0.05f);
            switch (CivilizationData.Name)
            {
                case "Mippip":
                    CivilizationState.AddFriendliness(-0.05f);
                    CivilizationState.AddTrust(-0.05f);
                    CivilizationState.AddInterest(-0.05f);
                    break;
                case "Handoull":
                    CivilizationState.AddInterest(-0.05f);
                    break;
                    
                case "Akki":
                    CivilizationState.AddInterest(-0.05f);
                    break;
                    
            }
        }
        
        private void OnMissionCompleted()
        {
            switch (CivilizationData.Name)
            {
                case "Handoull":
                    CivilizationState.AddTrust(0.10f);
                    CivilizationState.AddFriendliness(0.10f);
                    break;
                case "Halxi":
                    CivilizationState.AddTrust(0.10f);
                    CivilizationState.AddFriendliness(0.10f);
                    break;
            }
        }

        private void OnCivilizationDiscovered()
        {
            CivilizationState.AddInterest(0.1f);
            switch (CivilizationData.Name)
            {
                case "Akki":
                    CivilizationState.AddFriendliness(0.05f);
                    break;
                case "Handoull":
                    CivilizationState.AddTrust(-0.10f);
                    break;
            }
        }

        private void OnResearchCompleted(ResearchNode obj)
        {
            switch (CivilizationData.Name)
            {
                case "Akki":
                    CivilizationState.AddInterest(0.05f);
                    break;
                case "Handoull":
                    CivilizationState.AddTrust(0.10f);
                    break;
                case "Halxi":
                    CivilizationState.AddTrust(0.10f);
                    break;
            }
        }

        private void OnConstructibleCreated(ConstructibleDataSO obj)
        {
            switch (CivilizationData.Name)
            {
                case "Akki":
                    CivilizationState.AddInterest(0.05f);
                    break;
            }
        }

        private void OnPlanetAdded(Planet obj)
        {
            if(obj.PlanetData == CivilizationData.HomePlanetData)
            {
                CivilizationState.AddFriendliness(0.10f);
                CivilizationState.AddTrust(0.10f);
                CivilizationState.AddDependency(-0.15f);
            }

            switch (CivilizationData.Name)
            {
                case "Akki":
                    CivilizationState.AddInterest(0.05f);
                    break;
                case "Handoull":
                    if(obj.PlanetData != CivilizationData.HomePlanetData)
                    {
                        CivilizationState.AddInterest(-0.05f);
                    }
                    break;
            }
            UIEvents.OnUpdateCivilizationUI?.Invoke();
        }

        public string GetSaveId()
        {
            return $"Civilization_{CivilizationData.Name}";
        }

        public JToken CaptureState()
        {
            JObject obj = new JObject
            {
                ["CivilizationData"] = CivilizationData.CaptureState(),
                ["CivilizationState"] = CivilizationState.CaptureState(),
                ["StorageSystem"] = StorageSystem.CaptureState(),
                ["AIController"] = (int)AIControllerType
            };
            return obj;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = state as JObject;
            CivilizationData.RestoreState(obj["CivilizationData"]);
            CivilizationState.RestoreState(obj["CivilizationState"]);
            StorageSystem.RestoreState(obj["StorageSystem"]);
            AIControllerType = (AIType)obj["AIController"].ToObject<int>();
            AIController = AIControllerFactory.CreateAIController(AIControllerType, this, _invoker);
        }
    }
}