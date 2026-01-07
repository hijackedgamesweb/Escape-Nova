using BehaviourAPI.Core.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Systems.Behaviour.Perceptions
{
    public class WarLostPerception : Perception
    {
        private Planet _planet;
        public WarLostPerception(Planet planet) { _planet = planet; }

        public override bool Check()
        {
            if (_planet.AssociatedAI != null && _planet.AssociatedAI.PlayerSimulatedHealth <= 0)
            {
                return true;
            }
            return false;
        }
    }
}