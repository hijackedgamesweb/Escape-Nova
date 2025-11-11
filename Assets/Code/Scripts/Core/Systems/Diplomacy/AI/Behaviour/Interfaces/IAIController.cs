using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;

namespace Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces
{
    public interface IAIController
    {
        public void UpdateAI(WorldContext context);
        public void UpdateAI(WorldContext context, ICommand command);
        void SetCommandInvoker(CommandInvoker invoker);
    }
}