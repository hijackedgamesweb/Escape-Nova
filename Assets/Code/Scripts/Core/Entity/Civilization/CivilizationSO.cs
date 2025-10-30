using Code.Scripts.Core.Entity;
using Code.Scripts.Core.Systems.Civilization.AI.Behaviour.Interfaces;
using Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Civilization.ScriptableObjects
{
    public enum AIType
    {
        TestController,
    }
    public class CivilizationSO : EntitySO
    {
        [Header("Civilization Info")]
        public string civilizationDescription;
        public Sprite civilizationIcon;
        public Sprite civilizationFlag;
        
        [Header("Civilization Behaviour")]
        public AIType aiController;
        
        [Header("Civilization Attributes")]
        public float angerTolerance;
        
        [Header("Civilization Leader")]
        public Sprite leaderPortrait;
        
        
    }
}