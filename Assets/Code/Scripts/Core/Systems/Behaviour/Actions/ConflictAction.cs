using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour.Actions
{
    public class ConflictAction : Action
    {
        private Planet _planet;

        public ConflictAction(Planet planet)
        {
            _planet = planet;
        }

        public override void Start()
        {
            Debug.Log($"<color=red>[{_planet.Name}] FSM: ¡GUERRA INICIADA contra {_planet.Aggressor?.CivilizationData.Name}!</color>");
            // Visual: Activar alarmas, escudos rojos, partículas
        }

        public override Status Update()
        {
            if (_planet.AssociatedAI != null && _planet.AssociatedAI.WarHealth <= 0)
            {
                // Podríamos devolver Failure para indicar derrota, 
                // pero lo gestionaremos mejor con una Transición y Percepción.
            }
            return Status.Running;
        }

        public override void Stop()
        {
        }
    }
}