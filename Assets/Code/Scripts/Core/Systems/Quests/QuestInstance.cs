using System.Collections.Generic;
using Code.Scripts.Core.GameInfo;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Code.Scripts.Core.Systems.Quests
{
    [System.Serializable]
    public class QuestInstance : ISaveable
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
            
            if (questData.Objectives.Count == 0)
            {
            }
            
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
        
                if (questData.CompletionGameInfo != null)
                {
                    GameInfoManager.Instance.DisplayGameInfo(questData.CompletionGameInfo);
                }
                else
                {
                }
            }
        }

        public string GetSaveId()
        {
            return $"QuestInstance_{questData.QuestId}";
        }

        public JToken CaptureState()
        {
            var state = new JObject
            {
                ["questName"] = questData.QuestId,
                ["isActive"] = isActive,
                ["isCompleted"] = isCompleted,
                ["objectives"] = new JArray()
            };

            foreach (var objective in RuntimeObjectives)
            {
                var objectiveState = new JObject
                {
                    ["data"] = JToken.FromObject(objective.CaptureState())
                };
                ((JArray)state["objectives"]).Add(objectiveState);
            }

            return state;
        }

        public void RestoreState(JToken state)
        {
            isActive = state["isActive"].ToObject<bool>();
            isCompleted = state["isCompleted"].ToObject<bool>();
            JArray objectivesArray = (JArray)state["objectives"];
            RuntimeObjectives.Clear();

            foreach (var obj in objectivesArray)
            {
                string typeName = obj["type"].ToObject<string>();
                JToken data = obj["data"];

                var type = System.Type.GetType(typeName);
                if (type == null)
                {
                    Debug.LogError($"QuestObjective type '{typeName}' not found.");
                    continue;
                }

                var objective = (QuestObjective)System.Activator.CreateInstance(type);
                objective.RestoreState(data);
                RuntimeObjectives.Add(objective);
            }
        }
    }
}