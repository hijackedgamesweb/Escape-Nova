using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour.Actions
{
    public class DestroyedAction : Action
    {
        private Planet _planet;

        public DestroyedAction(Planet planet)
        {
            _planet = planet;
        }

        public override void Start()
        {
            Debug.Log($"<color=red>[{_planet.Name}] FSM: PLANETA DESTRUIDO</color>");

            // Visual: Cambiar sprite del planeta a escombros
            // var renderer = _planet.GetComponent<SpriteRenderer>();
            // renderer.sprite = Resources.Load<Sprite>("Sprites/Planets/Debris");

            // Lógica: Desactivar interacciones, parar producción...
            // _planet.StopProduction();
        }

        public override Status Update()
        {
            // Un planeta destruido se queda así para siempre (normalmente)
            return Status.Running;
        }

        public override void Stop() { }
    }
}