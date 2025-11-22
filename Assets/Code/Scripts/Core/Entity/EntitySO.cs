using UnityEngine;

namespace Code.Scripts.Core.Entity
{
    public class EntitySO : ScriptableObject
    {
        [Header("Entity Info")]
        public string civName;
        public string civLeaderName;
        
        [Header("Entity Base Attributes")]
        public float baseFriendship;
        public float baseDependency;
        public float baseInterest;
        public float baseTrust;
        
        [Header("Entity Stats")]
        public EntityItemPreferencesSO itemPreferences;
    }
}