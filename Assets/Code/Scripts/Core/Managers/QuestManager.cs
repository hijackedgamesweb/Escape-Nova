using System;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Quests;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private List<QuestData> allQuests;
        private List<QuestInstance> activeQuests = new();
        
        public event Action<QuestInstance> OnQuestCompleted;

        private void Awake()
        {
            ServiceLocator.RegisterService(this);
        }

        public void StartQuest(string questId)
        {
            var questData = allQuests.Find(q => q.QuestId == questId);
            if (questData != null)
            {
                var questInstance = new QuestInstance(questData);
                questInstance.isActive = true;
                activeQuests.Add(questInstance);
            }
            else
            {
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
                }
            }
        }
    }
}