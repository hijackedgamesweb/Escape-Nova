using System.Collections.Generic;
using Code.Scripts.Core.Systems.Quests.Objectives;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.ScriptableObjects
{
    [System.Serializable]
    public class QuestReward
    {
        public string Description; // E.g., "Madera", "Oro", "Experiencia"
        public int Amount;
        // Opcional: Podrías añadir "public Sprite Icon;" si tu prefab lo usa
    }

    [CreateAssetMenu(menuName = "Quest System/Quest Data")]
    public class QuestData : ScriptableObject
    {
        public string QuestId;
        public string Title;
        [TextArea] public string Description;
        [SerializeReference, SubclassSelector] public List<QuestObjective> Objectives;

        // --- LÍNEA ANTIGUA (Borrar o comentar) ---
        // public string Reward;

        // --- LÍNEA NUEVA ---
        public List<QuestReward> Rewards; 
    }
}