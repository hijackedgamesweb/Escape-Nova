using Code.Scripts.Core.Systems.Diplomacy.Actions;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;

namespace Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.TestController
{
    public class TestController : IAIController
    {
        CommandInvoker _invoker;
        WorldContext _worldContext;
        Entity.Civilization.Civilization _thisCiv;
        
        public TestController(Entity.Civilization.Civilization civ, CommandInvoker invoker)
        {
            _invoker = invoker;
            _thisCiv = civ;
        }
        
        public void ExecuteNextAction()
        {
            Entity.Entity targetEntity = _worldContext.Player;
            _invoker.ExecuteCommand(new Insultar(_thisCiv, targetEntity));
        }

        public void UpdateAI(WorldContext context)
        {
            _worldContext = context;
            if((int) context.CurrentTurn % 5 == 0)    
                ExecuteNextAction();
        }

        public void UpdateAI(WorldContext context, ICommand command)
        {
        }

        public void SetCommandInvoker(CommandInvoker invoker)
        {
            _invoker = invoker;
        }
    }
}