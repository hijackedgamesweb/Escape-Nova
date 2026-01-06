using BehaviourAPI.Core.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Systems.Behaviour.Perceptions
{
    public class PlanetDestroyedPerception : Perception
    {
        private Planet _planet;

        public PlanetDestroyedPerception(Planet planet)
        {
            _planet = planet;
        }

        public override bool Check()
        {
            return _planet.IsDestroyed;
        }
    }
}