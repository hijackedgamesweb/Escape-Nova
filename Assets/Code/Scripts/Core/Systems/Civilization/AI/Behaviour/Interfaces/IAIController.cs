using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;

namespace Code.Scripts.Core.Systems.Civilization.AI.Behaviour.Interfaces
{
    public interface IAIController
    {
        public void UpdateAI(WorldContext context);
        void SetCommandInvoker(CommandInvoker invoker);
    }
}