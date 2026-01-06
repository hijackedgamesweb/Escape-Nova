using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour.Actions
{
    public class ProducingAction : Action
    {
        private Planet _planet;

        public ProducingAction(Planet planet)
        {
            _planet = planet;
        }

        public override void Start()
        {
        }

        public override Status Update()
        {
            return Status.Running; 
        }

        public override void Stop() { }
    }
}