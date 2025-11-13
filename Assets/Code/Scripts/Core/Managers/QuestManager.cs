using System;
using System.Collections.Generic;
using System.Linq; // Necesario para .All()
using Code.Scripts.Core.Systems.Quests;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    [System.Serializable]
    public class QuestSet
    {
        public string setName; //solo organizacion
        public List<QuestData> quests;
    }

    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private List<QuestSet> allQuestSets = new();
        
        private List<QuestInstance> activeQuests = new();
        private int currentQuestSetIndex = 0;
        
        private List<QuestData> visibleQuests = new();

        public List<QuestData> VisibleQuests => visibleQuests;
        
        public event Action OnVisibleQuestsChanged;
        public event Action<QuestInstance> OnQuestCompleted;

        private void Awake()
        {
            ServiceLocator.RegisterService(this);
            LoadCurrentQuestSet();
        }

        public void StartQuest(string questId)
        {
            QuestData questData = null;
            
            foreach (var set in allQuestSets)
            {
                questData = set.quests.Find(q => q.QuestId == questId);
                if (questData != null) break;
            }

            if (questData != null)
            {
                if (activeQuests.Any(q => q.questData.QuestId == questId))
                {
                    return;
                }
                
                var questInstance = new QuestInstance(questData);
                questInstance.isActive = true;
                activeQuests.Add(questInstance);
            }
            else
            {
                Debug.LogError($"Quest con ID {questId} no encontrada en ningún lote.");
            }
        }
        
        private void Start()
        {
        }

        private void Update()
        {
            for (int i = activeQuests.Count - 1; i >= 0; i--)
            {
                var q = activeQuests[i];
                if (q.isCompleted) continue;
                
                q.CheckCompletion();
                
                if (q.isCompleted)
                {
                    OnQuestCompleted?.Invoke(q);
                    CheckForSetCompletion();
                }
            }
        }

        private void LoadCurrentQuestSet()
        {
            visibleQuests.Clear();

            if (currentQuestSetIndex < allQuestSets.Count)
            {
                var questsInSet = allQuestSets[currentQuestSetIndex].quests;
                
                visibleQuests.AddRange(questsInSet);

                // Auto-iniciamos todas las misiones de este lote
                foreach (var quest in questsInSet)
                {
                    StartQuest(quest.QuestId);
                }
            }
            
            OnVisibleQuestsChanged?.Invoke();
        }

        private void CheckForSetCompletion()
        {
            if (currentQuestSetIndex >= allQuestSets.Count)
            {
                return;
            }

            var currentSetQuests = allQuestSets[currentQuestSetIndex].quests;
            
            bool allComplete = currentSetQuests.All(questData => 
                activeQuests.Exists(instance => 
                    instance.questData.QuestId == questData.QuestId && instance.isCompleted
                )
            );

            if (allComplete)
            {
                Debug.Log($"Lote de misiones '{allQuestSets[currentQuestSetIndex].setName}' completado!");
                currentQuestSetIndex++;
                LoadCurrentQuestSet();
            }
        }
        
        /// <summary>
        /// Obtiene la lista de objetivos en tiempo real (RuntimeObjectives) de una QuestInstance activa.
        /// </summary>
        public List<QuestObjective> GetRuntimeObjectivesForQuest(string questId)
        {
            var questInstance = activeQuests.Find(q => q.questData.QuestId == questId);
            if (questInstance != null)
            {
                return questInstance.RuntimeObjectives;
            }
            
            Debug.LogWarning($"No se encontró QuestInstance para {questId}. Mostrando objetivos estáticos.");
            var questData = VisibleQuests.Find(q => q.QuestId == questId);
            if (questData != null)
            {
                return questData.Objectives;
            }
            
            return new List<QuestObjective>();
        }
    }
}