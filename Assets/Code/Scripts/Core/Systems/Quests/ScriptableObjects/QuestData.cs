using System.Collections.Generic;
using Code.Scripts.Core.Systems.Quests.Objectives;
using Code.Scripts.Core.GameInfo; // Asegúrate de importar esto donde esté tu GameInfoSO
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.ScriptableObjects
{
    [System.Serializable]
    public abstract class QuestReward
    {
        [TextArea] public string Description;
        public abstract void ApplyReward();
        public abstract string GetRewardInfo();
    }

    [CreateAssetMenu(menuName = "Quest System/Quest Data")]
    public class QuestData : ScriptableObject
    {
        public string QuestId;
        public string Title;
        [TextArea] public string Description;
        
        [Header("Completion Info (Optional)")]
        [Tooltip("Arrastra aquí el GameInfoSO si quieres mostrar un tutorial al acabar. Déjalo vacío si no.")]
        public GameInfoSO CompletionGameInfo;

        [SerializeReference, SubclassSelector] public List<QuestObjective> Objectives;
        [SerializeReference, SubclassSelector] public List<QuestReward> Rewards; 
    }
}