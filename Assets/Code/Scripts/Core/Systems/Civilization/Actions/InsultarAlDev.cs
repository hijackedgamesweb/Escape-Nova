using Code.Scripts.Patterns.Command.Interfaces;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Civilization.Actions
{
    public class InsultarAlDev : ICommand
    {
        Civilization _civilization;
        public InsultarAlDev(Civilization civilization)
        {
            _civilization = civilization;
        }
        public void Execute()
        {
            Debug.Log("El lider de los " + _civilization.CivilizationName + ", el gran "+ _civilization.LeaderName + "... dice que eres un pedazo de Gilipollas!");
        }

        public void Undo()
        {
            Debug.Log("Perdón, no era mi intención");
        }
    }
}