using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour
{
    [RequireComponent(typeof(Planet))]
    public class PlanetBehaviourRunner : BehaviourRunner
    {
        private Planet _planet;
        public PlanetFSM PlanetGraph { get; private set; }

        protected override BehaviourGraph CreateGraph()
        {
            
            _planet = GetComponent<Planet>();
            PlanetGraph = new PlanetFSM(_planet);
            
            return PlanetGraph;
        }
    }
}