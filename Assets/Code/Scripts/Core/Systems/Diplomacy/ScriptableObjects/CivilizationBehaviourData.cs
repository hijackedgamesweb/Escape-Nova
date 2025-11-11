using UnityEngine;

namespace Code.Scripts.Core.Systems.Diplomacy.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CivilizationBehaviourData", menuName = "Scriptable Objects/CivilizationBehaviourData")]
    public class CivilizationBehaviourData : ScriptableObject
    {
        public float Friendliness;
        public float Dependability;
        public float Interest;
        public float Trustworthiness;
    }
}
