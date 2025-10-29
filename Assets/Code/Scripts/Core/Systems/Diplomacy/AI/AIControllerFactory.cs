using Code.Scripts.Core.Systems.Civilization.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Civilization.ScriptableObjects;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.TestController;
using Code.Scripts.Patterns.Command;

namespace Code.Scripts.Core.Systems.Civilization.AI
{
    public static class AIControllerFactory
    {
        public static IAIController CreateAIController(AIType type, Entity.Civilization.Civilization civ, CommandInvoker invoker)
        {
            return type switch {
                AIType.TestController => new TestController(civ, invoker),
                _ => null
            };
        }
    }
}