using Code.Scripts.Patterns.Command.Interfaces;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Diplomacy.Actions
{
    public class Insultar : ICommand
    {
        Entity.Entity _fromCivilization;
        Entity.Entity _toCivilization;
        public Insultar(Entity.Entity fromCiv, Entity.Entity toCiv)
        {
            _fromCivilization = fromCiv;
            _toCivilization = toCiv;
        }
        public void Execute()
        {
            float tol = _toCivilization.EntityData.AngerTolerance;
            _toCivilization.EntityState.FriendlinessLevel += tol * 0.1f;
        }

        public void Undo()
        {
            Debug.Log("Perdón, no era mi intención");
        }
    }
}