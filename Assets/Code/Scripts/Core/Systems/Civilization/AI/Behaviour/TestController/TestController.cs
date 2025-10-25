using Code.Scripts.Core.Systems.Civilization.Actions;
using Code.Scripts.Core.Systems.Civilization.AI.Behaviour.Interfaces;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;

namespace Code.Scripts.Core.Systems.Civilization.AI.Behaviour.TestController
{
    public class TestController : IAIController
    {
        CommandInvoker _invoker;
        Civilization _civ;
        
        public TestController(Civilization civ, CommandInvoker invoker)
        {
            _invoker = invoker;
            _civ = civ;
        }
        
        public void ExecuteNextAction()
        {
            _invoker.ExecuteCommand(new InsultarAlDev(_civ));
        }

        public void UpdateAI(WorldContext context)
        {
            if((int) context.CurrentTurn % 5 == 0)    
                ExecuteNextAction();
        }

        public void SetCommandInvoker(CommandInvoker invoker)
        {
            _invoker = invoker;
        }
    }
}