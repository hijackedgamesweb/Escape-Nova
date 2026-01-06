using BehaviourAPI.Core.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Systems.Behaviour.Perceptions
{
    public class WarDeclaredPerception : Perception
    {
        private Planet _planet;
        public WarDeclaredPerception(Planet planet) { _planet = planet; }

        public override bool Check()
        {
            // Comprobamos si hay un agresor asignado y tenemos IA asociada
            return _planet.Aggressor != null && _planet.AssociatedAI != null && _planet.AssociatedAI._isAtWarWithPlayer;
        }
    }
}