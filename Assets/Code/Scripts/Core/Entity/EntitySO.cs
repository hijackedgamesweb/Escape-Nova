using UnityEngine;

namespace Code.Scripts.Core.Entity
{
    public class EntitySO : ScriptableObject
    {
        [Header("Entity Info")]
        public string civName;
        public string civLeaderName;
        
        [Header("Entity Base Attributes")]
        public float baseHunger;
        public float baseAnger;
        public float baseMilitaryPower;
    }
}