using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.UI.Menus;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour.Actions
{
    public class ColonizedAction : Action
    {
        private Planet _planet;

        public ColonizedAction(Planet planet)
        {
            _planet = planet;
        }

        public override void Start()
        {
            Debug.Log($"[{_planet.Name}] FSM: Estado COLONIZADO por {_planet.Owner?.CivilizationData.Name}");
        }

        public override Status Update()
        {
            return Status.Running;
        }
    }
}