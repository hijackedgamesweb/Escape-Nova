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

        [Header("Animation Configuration")]
        public string producingAnim = "Producing";
        public string colonizedAnim = "Colonized";
        public string conflictAnim = "Conflict";
        public string conqueredAnim = "Conquered";
        public string destroyedAnim = "Destroyed";
        
        protected override BehaviourGraph CreateGraph()
        {
            
            _planet = GetComponent<Planet>();
            PlanetGraph = new PlanetFSM(_planet, this);
            
            return PlanetGraph;
        }

        //protected override void OnUpdated()
        //{
        //    base.OnUpdated();
//
        //    if (PlanetGraph != null)
        //    {
        //        string stateName = PlanetGraph.GetCurrentStateName(); 
        //        Debug.Log($"[{name}] FSM Status: {PlanetGraph.Status} | Current State: {stateName}");
        //    }
        //}
    }
}