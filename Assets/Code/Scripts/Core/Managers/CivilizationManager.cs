using System.Collections.Generic;
using Code.Scripts.Core.Systems.Civilization;
using Code.Scripts.Core.World;

namespace Code.Scripts.Core.Managers
{
    public class CivilizationManager
    {
        List<Civilization> _civilizations = new();
        
        public void AddCivilization(Civilization civ)
        {
            _civilizations.Add(civ);
        }
        
        public void UpdateCivilizations(WorldContext context)
        {
            foreach (var civ in _civilizations)
            {
                civ.AIController.UpdateAI(context);
            }
        }
    }
}