using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Code.Scripts.Core.Systems.Quests
{
    [System.Serializable]
    public class QuestInstance
    {
        public QuestData questData; 
        public bool isActive;
        public bool isCompleted;
        public List<QuestObjective> RuntimeObjectives;
        
        public QuestInstance(QuestData questData)
        {
            this.questData = questData;
            isActive = false;
            isCompleted = false;
            RuntimeObjectives = new List<QuestObjective>();
            foreach (var objective in questData.Objectives)
            {
                var json = JsonUtility.ToJson(objective);
                var runtimeObjective = (QuestObjective)JsonUtility.FromJson(json, objective.GetType());

                runtimeObjective.Initialize();
                runtimeObjective.RegisterEvents();
                RuntimeObjectives.Add(runtimeObjective);
            }
            
        }
        
        public void CheckCompletion()
        {
            isCompleted = RuntimeObjectives.TrueForAll(o => o.isCompleted);
            if (isCompleted)
            {
                foreach (var objective in RuntimeObjectives)
                {
                    objective.UnregisterEvents();
                }
                NotificationManager.Instance.CreateNotification($"Quest Completed: {questData.Title}", NotificationType.Info);
            }
        }

    }
}