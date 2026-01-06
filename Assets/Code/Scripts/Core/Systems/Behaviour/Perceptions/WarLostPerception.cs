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
            // Si estamos en guerra y la vida bajó a 0
            if (_planet.AssociatedAI != null && _planet.AssociatedAI.WarHealth <= 0)
            {
                return true;
            }
            return false;
        }
    }
}