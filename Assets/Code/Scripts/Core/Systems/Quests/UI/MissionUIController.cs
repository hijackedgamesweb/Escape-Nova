using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using Code.Scripts.Core.Systems.Quests; // Para QuestObjective
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.UI
{
    public class MissionsUIController : MonoBehaviour
    {
        // --- REFERENCIAS DE LA UI (Arrastrar en el Inspector) ---

        [Header("Prefabs")] [SerializeField] private GameObject missionButtonPrefab; // Tu prefab 'MisionBtn'
        [SerializeField] private GameObject subtaskPrefab; // Tu prefab 'SubtaskPrefab'
        [SerializeField] private GameObject recompensaPrefab; // Tu prefab 'RecompensaPrefab'

        [Header("Contenedores")] [SerializeField]
        private Transform missionButtonContainer; // 'Content' (hijo de Scroll View)

        [SerializeField] private Transform subtaskContainer; // Donde se instancian las subtareas
        [SerializeField] private Transform recompensaContainer; // 'Recompensa' (donde van los prefabs)

        [Header("Panel Central (Info)")] [SerializeField]
        private TMP_Text missionNameText; // 'MisionName'

        [SerializeField] private TMP_Text timeContentText; // 'TimeContent/Text (TMP)'

        [Header("Panel Derecho (Detalles)")] [SerializeField]
        private TMP_Text questDescriptionText; // El TMP_Text de 'QuestDescription'

        [SerializeField] private TMP_Text recompensaHeaderText; // 'Recompensa/RecompensaText'

        // --- Referencias Internas ---
        private QuestManager questManager;
        private List<GameObject> activeSubtaskObjects = new List<GameObject>();
        private List<GameObject> activeRewardObjects = new List<GameObject>();

        void Start()
        {
            questManager = ServiceLocator.GetService<QuestManager>();

            if (questManager == null)
            {
                Debug.LogError("QuestManager no encontrado en ServiceLocator!");
                return;
            }

            PopulateMissionList();

            // Mostrar la primera misión por defecto
            if (questManager.AllQuests.Count > 0)
            {
                DisplayQuestDetails(questManager.AllQuests[0]);
            }
        }

        // --- 1. Poblar la lista de botones de misión ---
        private void PopulateMissionList()
        {
            foreach (Transform child in missionButtonContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (QuestData quest in questManager.AllQuests)
            {
                GameObject buttonObj = Instantiate(missionButtonPrefab, missionButtonContainer);
                MissionButton missionButton = buttonObj.GetComponent<MissionButton>();

                if (missionButton != null)
                {
                    missionButton.Setup(quest, this);
                }
            }
        }

        // --- 2. Mostrar detalles (llamado por MissionButton.cs) ---
        public void DisplayQuestDetails(QuestData quest)
        {
            if (quest == null) return;

            // --- Actualizar Panel Central ---
            missionNameText.text = quest.Title;

            // Punto 2: Placeholder para los ciclos, como pediste.
            timeContentText.text = "??? Ciclos";

            // --- Actualizar Subtareas ---
            foreach (GameObject subtaskObj in activeSubtaskObjects)
            {
                Destroy(subtaskObj);
            }

            activeSubtaskObjects.Clear();

            // Punto 3: Usamos 'objectiveDescription'
            foreach (QuestObjective objective in quest.Objectives)
            {
                GameObject subtaskObj = Instantiate(subtaskPrefab, subtaskContainer);
                SubtaskUIItem uiItem = subtaskObj.GetComponent<SubtaskUIItem>();
                if (uiItem != null)
                {
                    uiItem.Setup(objective); // Llama al script actualizado
                }

                activeSubtaskObjects.Add(subtaskObj);
            }

            // --- Actualizar Panel Derecho ---
            questDescriptionText.text = quest.Description;

            // Punto 4: Lógica de lista de recompensas
            recompensaHeaderText.text = "Recompensas:"; // Texto fijo

            // Limpiar recompensas anteriores
            foreach (GameObject rewardObj in activeRewardObjects)
            {
                Destroy(rewardObj);
            }

            activeRewardObjects.Clear();

            // Instanciar los prefabs de recompensa
            foreach (QuestReward reward in quest.Rewards)
            {
                GameObject rewardObj = Instantiate(recompensaPrefab, recompensaContainer);
                RewardUIItem uiItem = rewardObj.GetComponent<RewardUIItem>();

                if (uiItem != null)
                {
                    uiItem.Setup(reward); // Llama al script nuevo
                }

                activeRewardObjects.Add(rewardObj);
            }
        }
    }
}