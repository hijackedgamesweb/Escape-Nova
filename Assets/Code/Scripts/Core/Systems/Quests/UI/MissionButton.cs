using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Quests.UI
{
    public class MissionButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text missionTitleText;
    
        private QuestData assignedQuest;
        private MissionsUIController uiController;

        private void Awake()
        {
            // Añadir el listener al botón
            GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        // El controlador principal llamará a esto al crear el botón
        public void Setup(QuestData questData, MissionsUIController controller)
        {
            assignedQuest = questData;
            uiController = controller;
            missionTitleText.text = assignedQuest.Title;
        }

        private void OnButtonClick()
        {
            // Le dice al controlador principal que muestre los detalles de esta misión
            uiController.DisplayQuestDetails(assignedQuest);
        }
    }
}