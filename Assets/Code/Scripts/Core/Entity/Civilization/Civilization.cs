using Code.Scripts.Core.Systems.Civilization.AI;
using Code.Scripts.Core.Systems.Civilization.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Civilization.ScriptableObjects;
using Code.Scripts.Patterns.Command;

namespace Code.Scripts.Core.Entity.Civilization
{
    public class Civilization : Entity
    {
        public CivilizationData CivilizationData {get; private set; }
        public CivilizationState CivilizationState  {get; private set; }
        private CommandInvoker _invoker;
        public IAIController AIController { get; private set; }
        
        public Civilization(CommandInvoker invoker, CivilizationSO civilizationSO) : base(invoker, civilizationSO)
        {
            _invoker = invoker;
            CivilizationData = new CivilizationData(civilizationSO);
            CivilizationState = new CivilizationState(civilizationSO);
            AIController = AIControllerFactory.CreateAIController(civilizationSO.aiController, this, invoker);
        }
    }
}