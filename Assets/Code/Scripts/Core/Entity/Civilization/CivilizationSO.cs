using UnityEngine;

namespace Code.Scripts.Core.Entity.Civilization
{
    public enum AIType
    {
        TestController,
        AkkiBehaviour,
        HalxiBehaviour,
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