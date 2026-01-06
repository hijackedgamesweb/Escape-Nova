using BehaviourAPI.Core.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Systems.Behaviour.Perceptions
{
    public class HasOwnerPerception : Perception
    {
        private Planet _planet;
        public HasOwnerPerception(Planet planet) { _planet = planet; }

        public override bool Check()
        {
            return _planet.Owner != null;
        }
    }
}