using BehaviourAPI.Core.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Systems.Behaviour.Perceptions
{
    public class WarWonPerception : Perception
    {
        private Planet _planet;
        public WarWonPerception(Planet planet) { _planet = planet; }

        public override bool Check()
        {
            if (_planet.AssociatedAI != null && !_planet.AssociatedAI._isAtWarWithPlayer)
            {
                return true;
            }
            return false;
        }
    }
}