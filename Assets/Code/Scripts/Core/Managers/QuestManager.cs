using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Core.SaveLoad.Interfaces; // Necesario para .All()
using Code.Scripts.Core.Systems.Quests;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    [System.Serializable]
    public class QuestSet
    {
        public string setName; //solo organizacion
        public List<QuestData> quests;
    }

    public class QuestManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private List<QuestSet> allQuestSets = new();
        
        private List<QuestInstance> activeQuests = new();
        private int currentQuestSetIndex = 0;
        
        private List<QuestData> visibleQuests = new();
        private List<QuestData> completedQuests = new();

        public List<QuestData> VisibleQuests => visibleQuests;
        public List<QuestData> CompletedQuests => completedQuests;
         
        public event Action OnVisibleQuestsChanged;
        public event Action<QuestInstance> OnQuestCompleted;

        private void Awake()
        {
            ServiceLocator.RegisterService(this);
            LoadCurrentQuestSet();
        }

        public void StartQuest(string questId, bool[] isObjectiveCompleted = null)
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
                
                if(isObjectiveCompleted != null)
                {
                    for (int i = 0; i < isObjectiveCompleted.Length && i < questInstance.RuntimeObjectives.Count; i++)
                    {
                        questInstance.RuntimeObjectives[i].isCompleted = isObjectiveCompleted[i];
                    }
                }
                
                if (completedQuests.Contains(questData))
                {
                    questInstance.isCompleted = true;
                    foreach (var objective in questInstance.RuntimeObjectives)
                    {
                        objective.isCompleted = true;
                    }
                }
                activeQuests.Add(questInstance);
            }
            else
            {
            }
        }
        
        public void StartSecondaryQuest(QuestData questData)
        {
            if (activeQuests.Any(q => q.questData.QuestId == questData.QuestId))
            {
                return;
            }
                
            var questInstance = new QuestInstance(questData);
            questInstance.isActive = true;
            activeQuests.Add(questInstance);
            visibleQuests.Add(questData);
            OnVisibleQuestsChanged?.Invoke();
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
                    foreach (var reward in q.questData.Rewards)
                    {
                        reward.ApplyReward();
                    }
                    completedQuests.Add(q.questData);
                    visibleQuests.Remove(q.questData);
                    OnQuestCompleted?.Invoke(q);
                    CheckForSetCompletion();
                }
            }
        }

        private void LoadCurrentQuestSet()
        {
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

        private void LoadQuests(JArray completedObjectivesArray)
        {
            visibleQuests.Clear();
            activeQuests.Clear();
            for (int i = currentQuestSetIndex; i >= 0; i--)
            {
                var questsInSet = allQuestSets[i].quests;
                if (i != currentQuestSetIndex)
                {
                    foreach (var quest in questsInSet)
                    {
                        completedQuests.Add(quest);
                        StartQuest(quest.QuestId);
                    }
                }
                else
                {
                    foreach (var quest in questsInSet)
                    {
                        JToken questState = null;
                        foreach (var item in completedObjectivesArray)
                        {
                            if (item["questName"].ToObject<string>() == quest.QuestId)
                            {
                                questState = item;
                                break;
                            }
                        }
                        if (questState != null)
                        {
                            JArray objectivesArray = (JArray)questState["objectives"];
                            bool[] isObjectiveCompleted = new bool[objectivesArray.Count];
                            for (int j = 0; j < objectivesArray.Count; j++)
                            {
                                isObjectiveCompleted[j] = objectivesArray[j]["data"]["isCompleted"].ToObject<bool>();
                            }
                            StartQuest(quest.QuestId, isObjectiveCompleted);
                        }
                        visibleQuests.Add(quest);
                    }
                }
            }
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
                currentQuestSetIndex++;
                LoadCurrentQuestSet();
            }
        }
        
        public List<QuestObjective> GetRuntimeObjectivesForQuest(string questId)
        {
            var questInstance = activeQuests.Find(q => q.questData.QuestId == questId);
            if (questInstance != null)
            {
                return questInstance.RuntimeObjectives;
            }
            
            var questData = VisibleQuests.Find(q => q.QuestId == questId);
            if (questData != null)
            {
                return questData.Objectives;
            }
            
            return new List<QuestObjective>();
        }

        public string GetSaveId()
        {
            return "QuestManager";
        }

        public JToken CaptureState()
        {
            JObject obj = new JObject
            {
                ["currentQuestSetIndex"] = currentQuestSetIndex,
                ["activeQuests"] = new JArray(activeQuests.Select(q => q.CaptureState())),
                ["completedQuests"] = new JArray(completedQuests.Select(q => q.QuestId))
            };
            return obj;
        }

        public void RestoreState(JToken state)
        {
            currentQuestSetIndex = state["currentQuestSetIndex"].ToObject<int>();
            var completedObjectivesArray = state["activeQuests"] as JArray;

            LoadQuests(completedObjectivesArray);
        }

        private void InitializeLoadQuest()
        {
            
        }
    }
}