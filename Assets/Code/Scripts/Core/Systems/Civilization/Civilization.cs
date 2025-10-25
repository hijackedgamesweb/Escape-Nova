using Code.Scripts.Core.Systems.Civilization.AI;
using Code.Scripts.Core.Systems.Civilization.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Civilization.Interfaces;
using Code.Scripts.Core.Systems.Civilization.ScriptableObjects;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Civilization
{
    public class Civilization : ICivilization
    {
        public string CivilizationName { get; private set; }
        public string LeaderName { get; private set; }
        
        private CommandInvoker _invoker;
        public IAIController AIController { get; private set; }
        
        public Civilization(CommandInvoker invoker, CivilizationSO civilizationSO)
        {
            _invoker = invoker;
            CivilizationName = civilizationSO.civilizationName;
            LeaderName = civilizationSO.leaderName;
            AIController = AIControllerFactory.CreateAIController(civilizationSO.aiController, this, invoker);
        }
    }
}