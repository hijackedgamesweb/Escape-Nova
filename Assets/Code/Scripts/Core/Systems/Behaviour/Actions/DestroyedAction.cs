using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour.Actions
{
    public class DestroyedAction : Action
    {
        private Planet _planet;
        private string _animName;
        private float _timer = 0f;
        private float _animationLength = 1f;

        public DestroyedAction(Planet planet, string animName)
        {
            _planet = planet;
            _animName = animName;
        }

        public override void Start()
        {
            Debug.Log($"<color=red>[{_planet.Name}] FSM: Ejecutando DestroyedAction ({_animName})</color>");
            
            var spriteRenderer = _planet.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            var collider = _planet.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            var animator = _planet.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Play(_animName);
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(_animName))
                    _animationLength = stateInfo.length;
            }
        }

        public override Status Update()
        {
            _timer += UnityEngine.Time.deltaTime;
            if (_timer >= _animationLength)
            {
                var solarSystem = ServiceLocator.GetService<SolarSystem>();
                if (solarSystem != null)
                {
                    solarSystem.RemovePlanet(_planet.OrbitIndex, _planet.PlanetIndex);
                }
                return Status.Success;
            }
            return Status.Running;
        }
    }
}