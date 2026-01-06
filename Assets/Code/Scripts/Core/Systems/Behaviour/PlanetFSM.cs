using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using Code.Scripts.Core.Systems.Behaviour.Actions;
using Code.Scripts.Core.Systems.Behaviour.Perceptions;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Systems.Behaviour
{
    public class PlanetFSM : FSM
    {
        // Exponemos los estados para que el Runner y el Planet puedan leerlos
        public State ProducingState { get; private set; }
        public State ColonizedState { get; private set; }
        public State ConflictState { get; private set; }
        public State ConqueredState { get; private set; }
        public State DestroyedState { get; private set; }

        public PlanetFSM(Planet planet)
        {
            // 1. CREAR NODOS (ESTADOS)
            
            ProducingState = CreateNode<State>("Producing");
            ProducingState.Action = new ProducingAction(planet);

            ColonizedState = CreateNode<State>("Colonized");
            ColonizedState.Action = new ColonizedAction(planet);

            ConflictState = CreateNode<State>("Conflict");
            ConflictState.Action = new ConflictAction(planet);

            ConqueredState = CreateNode<State>("Conquered");
            ConqueredState.Action = new ConqueredAction(planet);

            DestroyedState = CreateNode<State>("Destroyed");
            DestroyedState.Action = new DestroyedAction(planet);

            // Source -> Transition -> Target

            // A. Producing -> Colonized
            CreateTransition(ProducingState, ColonizedState, new HasOwnerPerception(planet));

            // B. Colonized -> Conflict
            CreateTransition(ColonizedState, ConflictState, new WarDeclaredPerception(planet));

            // C. Conflict -> Conquered
            CreateTransition(ConflictState, ConqueredState, new WarLostPerception(planet));

            // D. Conflict -> Colonized (Paz)
            CreateTransition(ConflictState, ColonizedState, new WarWonPerception(planet));

            // E. Transiciones a Destroyed
            CreateTransition(ConflictState, DestroyedState, new PlanetDestroyedPerception(planet));
            CreateTransition(ColonizedState, DestroyedState, new PlanetDestroyedPerception(planet));
            CreateTransition(ConqueredState, DestroyedState, new PlanetDestroyedPerception(planet));

            // 3. ESTADO INICIAL
            SetCurrentState(ProducingState, null);
        }

        private void CreateTransition(State from, State to, Perception perception)
        {
            // Creamos el nodo de transición
            var transition = CreateNode<StateTransition>();
            transition.Perception = perception;

            // Origen -> Transición
            Connect(from, transition);

            // Transición -> Destino
            // StateTransition.BuildConnections usa el primer hijo como target
            Connect(transition, to);
        }
    }
}