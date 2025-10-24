using Code.Scripts.Core.Systems.Civilization.AI.Behaviour.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Civilization.ScriptableObjects
{
    public enum AIType
    {
        TestController,
    }
    public class CivilizationSO : ScriptableObject
    {
        [Header("Civilization Info")]
        public string civilizationName;
        public string civilizationDescription;
        public Image civilizationIcon;
        public Image civilizationFlag;
        
        [Header("Civilization Behaviour")]
        public AIType aiController;
        
        [Header("Civilization Attributes")]
        
        
        [Header("Civilization Leader")]
        public string leaderName;
        public Image leaderPortrait;
        
        
    }
}