using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.TestController;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Patterns.Command;

namespace Code.Scripts.Core.Systems.Diplomacy.AI
{
    public static class AIControllerFactory
    {
        public static IAIController CreateAIController(AIType type, Entity.Civilization.Civilization civ, CommandInvoker invoker)
        {
            return type switch {
                AIType.TestController => new TestController(civ, invoker),
                AIType.AkkiBehaviour => new AkkiBehaviour(civ, invoker),
                AIType.HalxiBehaviour => new HalxiBehaviour(civ, invoker),
                _ => null
            };
        }
    }
}