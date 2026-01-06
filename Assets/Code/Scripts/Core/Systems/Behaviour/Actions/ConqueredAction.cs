using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour.Actions
{
    public class ConqueredAction : Action
    {
        private Planet _planet;

        public ConqueredAction(Planet planet)
        {
            _planet = planet;
        }

        public override void Start()
        {
            // Lógica visual: Cambiar banderas al nuevo conquistador, humo leve, etc.
            Debug.Log($"[{_planet.Name}] FSM: El planeta ha sido CONQUISTADO por {_planet.Owner?.CivilizationData.Name}");
            
            // Opcional: Reducir la producción del planeta por daños de guerra
            // _planet.ApplyProductionPenalty(0.5f); 
        }

        public override Status Update()
        {
            // El estado se mantiene activo indefinidamente hasta que pase algo nuevo
            return Status.Running;
        }

        public override void Stop()
        {
            // Limpieza si el planeta deja de estar conquistado (ej: reconquista)
        }
    }
}