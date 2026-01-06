using BehaviourAPI.Core.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour.Perceptions
{
    public class WarDeclaredPerception : Perception
    {
        private Planet _planet;

        public WarDeclaredPerception(Planet planet)
        {
            _planet = planet;
        }

        public override bool Check()
        {
            if (_planet.AssociatedAI == null) return false;
            if (_planet.AssociatedAI._isAtWarWithPlayer) return true;
            if (_planet.Aggressor != null) return true;

            return false;
        }
    }
}