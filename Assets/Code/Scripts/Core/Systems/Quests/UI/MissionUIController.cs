using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Quests.UI
{
    public class MissionsUIController : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] private GameObject missionButtonPrefab;
        [SerializeField] private GameObject subtaskPrefab;
        [SerializeField] private GameObject recompensaPrefab;
        [Header("Contenedores")] 
        [SerializeField] private Transform missionButtonContainer;
        [SerializeField] private Transform subtaskContainer;
        [SerializeField] private Transform recompensaContainer;

        [Header("Panel Central (Info)")] [SerializeField]
        private TMP_Text missionNameText;
        [SerializeField] private TMP_Text timeContentText;

        [Header("Panel Derecho (Detalles)")] [SerializeField]
        private TMP_Text questDescriptionText;
        [SerializeField] private TMP_Text recompensaHeaderText;

        // --- Referencias Internas ---
        private QuestManager questManager;
        private List<GameObject> activeSubtaskObjects = new List<GameObject>();
        private List<GameObject> activeRewardObjects = new List<GameObject>();
        
        // Guardamos los scripts de item, no solo los GameObjects
        private List<SubtaskUIItem> activeSubtaskItems = new List<SubtaskUIItem>();

        private QuestData selectedQuest = null;

        void Start()
        {
            questManager = ServiceLocator.GetService<QuestManager>();

            if (questManager == null)
            {
                Debug.LogError("QuestManager no encontrado. El panel de misiones no funcionará.");
                return;
            }

            questManager.OnVisibleQuestsChanged += HandleQuestListChanged;
            PopulateMissionList();

            if (questManager.VisibleQuests.Count > 0)
            {
                DisplayQuestDetails(questManager.VisibleQuests[0]);
            }
            else
            {
                ClearQuestDetails();
            }
        }

        private void HandleQuestListChanged()
        {
            PopulateMissionList();
            
            if (questManager.VisibleQuests.Count > 0)
            {
                DisplayQuestDetails(questManager.VisibleQuests[0]);
            }
            else
            {
                ClearQuestDetails();
            }
        }

        private void PopulateMissionList()
        {
            foreach (Transform child in missionButtonContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (QuestData quest in questManager.VisibleQuests)
            {
                GameObject buttonObj = Instantiate(missionButtonPrefab, missionButtonContainer);
                MissionButton missionButton = buttonObj.GetComponent<MissionButton>();

                if (missionButton != null)
                {
                    missionButton.Setup(quest, this);
                }
                
                if (selectedQuest != null && quest.QuestId == selectedQuest.QuestId)
                {
                    missionButton.SetSelected(true);
                }
            }
        }
        
        public void DisplayQuestDetails(QuestData quest)
        {
            if (quest == null)
            {
                ClearQuestDetails();
                return;
            }

            selectedQuest = quest;
            
            foreach (Transform child in missionButtonContainer)
            {
                MissionButton mb = child.GetComponent<MissionButton>();
                if (mb != null)
                {
                    mb.SetSelected(mb.AssignedQuest != null && mb.AssignedQuest.QuestId == selectedQuest.QuestId);
                }
            }

            missionNameText.text = quest.Title;
            timeContentText.text = "OBJECTIVES"; 
            
            // Limpiar subtareas
            foreach (GameObject subtaskObj in activeSubtaskObjects)
            {
                Destroy(subtaskObj);
            }
            activeSubtaskObjects.Clear();
            activeSubtaskItems.Clear(); // Limpiamos la lista de scripts también

            // Obtenemos los objetivos de la INSTANCIA (runtime)
            List<QuestObjective> runtimeObjectives = questManager.GetRuntimeObjectivesForQuest(quest.QuestId);
            
            // Poblar subtareas
            foreach (QuestObjective objective in runtimeObjectives) 
            {
                GameObject subtaskObj = Instantiate(subtaskPrefab, subtaskContainer);
                SubtaskUIItem uiItem = subtaskObj.GetComponent<SubtaskUIItem>();
                if (uiItem != null)
                {
                    uiItem.Setup(objective); 
                    activeSubtaskItems.Add(uiItem); // Añadimos el script a la lista
                }
                activeSubtaskObjects.Add(subtaskObj); 
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(subtaskContainer as RectTransform);

            questDescriptionText.text = quest.Description;
            recompensaHeaderText.text = "Rewards:";
            
            foreach (GameObject rewardObj in activeRewardObjects)
            {
                Destroy(rewardObj);
            }
            activeRewardObjects.Clear();

            foreach (QuestReward reward in quest.Rewards)
            {
                GameObject rewardObj = Instantiate(recompensaPrefab, recompensaContainer);
                RewardUIItem uiItem = rewardObj.GetComponent<RewardUIItem>();

                if (uiItem != null)
                {
                    uiItem.Setup(reward);
                }

                activeRewardObjects.Add(rewardObj);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(recompensaContainer as RectTransform);
        }

        private void ClearQuestDetails()
        {
            selectedQuest = null;

            missionNameText.text = "Sin Misiones";
            timeContentText.text = "-";
            questDescriptionText.text = "No hay misiones disponibles en este momento.";
            recompensaHeaderText.text = "Recompensas:";

            foreach (GameObject subtaskObj in activeSubtaskObjects)
            {
                Destroy(subtaskObj);
            }
            activeSubtaskObjects.Clear();
            activeSubtaskItems.Clear(); // Añadido: Limpiar la lista de scripts

            foreach (GameObject rewardObj in activeRewardObjects)
            {
                Destroy(rewardObj);
            }
            activeRewardObjects.Clear();
            
            foreach (Transform child in missionButtonContainer)
            {
                MissionButton mb = child.GetComponent<MissionButton>();
                if (mb != null)
                {
                    mb.SetSelected(false);
                }
            }
        }
        
        // Método Update para refrescar los ticks de completado en vivo
        void Update()
        {
            if (selectedQuest == null || activeSubtaskItems.Count == 0)
            {
                return;
            }

            foreach (var item in activeSubtaskItems)
            {
                item.UpdateVisuals();
            }
        }

        private void OnDestroy()
        {
            if (questManager != null)
            {
                questManager.OnVisibleQuestsChanged -= HandleQuestListChanged;
            }
        }
    }
}