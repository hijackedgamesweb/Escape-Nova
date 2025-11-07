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
                Debug.LogWarning($"Quest with ID {questId} not found.");
            }
        }

        private void Start()
        {
            StartQuest("pruebaMision");
        }


        private void Update()
        {
            foreach (var q in activeQuests)
            {
                if(!q.isCompleted)
                {
                    q.CheckCompletion();
                }
            }
        }
    }
}