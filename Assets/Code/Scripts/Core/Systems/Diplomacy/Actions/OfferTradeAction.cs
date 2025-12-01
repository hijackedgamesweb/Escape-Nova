using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;

namespace Code.Scripts.Core.Systems.Diplomacy.Actions
{
    public class OfferTradeAction : FunctionalAction
    {
        public OfferTradeAction(Entity.Civilization.Civilization civ) : base(StartAction, UpdateAction, EndAction)
        {
            
        }

        public static void StartAction()
        {
            
        }
        
        public static Status UpdateAction()
        {
            return Status.Running;
        }
        
        public static void EndAction()
        {
            
        }
    }
}