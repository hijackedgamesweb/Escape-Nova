using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using Code.Scripts.Core.Systems.Behaviour.Actions;
using Code.Scripts.Core.Systems.Behaviour.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Behaviour
{
    public class PlanetFSM : FSM
    {
        public State ProducingState { get; private set; }
        public State ColonizedState { get; private set; }
        public State ConflictState { get; private set; }
        public State ConqueredState { get; private set; }
        public State DestroyedState { get; private set; }

        public PlanetFSM(Planet planet, PlanetBehaviourRunner config)
        {
            ProducingState = CreateNode<State>("Producing");
            ProducingState.Action = new ProducingAction(planet, config.producingAnim);

            ColonizedState = CreateNode<State>("Colonized");
            ColonizedState.Action = new ColonizedAction(planet, config.colonizedAnim);

            ConflictState = CreateNode<State>("Conflict");
            ConflictState.Action = new ConflictAction(planet, config.conflictAnim);

            ConqueredState = CreateNode<State>("Conquered");
            ConqueredState.Action = new ConqueredAction(planet, config.conqueredAnim);

            DestroyedState = CreateNode<State>("Destroyed");
            DestroyedState.Action = new DestroyedAction(planet, config.destroyedAnim);

            CreateTransition(ProducingState, ColonizedState, new HasOwnerPerception(planet));
            CreateTransition(ColonizedState, ConflictState, new WarDeclaredPerception(planet));
            CreateTransition(ConflictState, ConqueredState, new WarLostPerception(planet));
            CreateTransition(ConflictState, ColonizedState, new WarWonPerception(planet));

            CreateTransition(ProducingState, DestroyedState, new PlanetDestroyedPerception(planet));
            CreateTransition(ColonizedState, DestroyedState, new PlanetDestroyedPerception(planet));
        }

        protected override void OnStarted()
        {
            if (_currentState == null)
            {
                SetCurrentState(ProducingState, null);
            }
        }

        private void CreateTransition(State from, State to, Perception perception)
        {
            base.CreateTransition(
                from, 
                to, 
                perception, 
                null,
                StatusFlags.Running | StatusFlags.Success | StatusFlags.Failure
            );
        }
        
        public string GetCurrentStateName()
        {
            if (_currentState == null) return "NULL";
            foreach (var pair in GetNodeNames())
            {
                if (pair.Key == _currentState) return pair.Value;
            }
            return "Unknown";
        }
    }
}