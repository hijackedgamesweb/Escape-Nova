using Code.Scripts.Core.Systems.Civilization.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Civilization.ScriptableObjects;
using Code.Scripts.Patterns.Command;

namespace Code.Scripts.Core.Systems.Civilization.AI
{
    public static class AIControllerFactory
    {
        public static IAIController CreateAIController(AIType type, Civilization civ, CommandInvoker invoker)
        {
            return type switch {
                AIType.TestController => new Behaviour.TestController.TestController(civ, invoker),
                _ => null
            };
        }
    }
}