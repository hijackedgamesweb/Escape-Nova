using BehaviourAPI.Core.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

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
            if (_planet.IsDestroyed) 
            {
                Debug.Log($"<color=yellow>[PERCEPTION] {_planet.Name}: ¡Detectado IsDestroyed = TRUE! Debería transicionar ya.</color>");
            }
            
            return _planet.IsDestroyed;
        }
    }
}