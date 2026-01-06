using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour.Actions
{
    public class ProducingAction : Action
    {
        private Planet _planet;
        private string _animName;

        public ProducingAction(Planet planet, string animName)
        {
            _planet = planet;
        }

        public override void Start()
        {
            var animator = _planet.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play(_animName);
            }
        }

        public override Status Update()
        {
            return Status.Running; 
        }

        public override void Stop() { }
    }
}