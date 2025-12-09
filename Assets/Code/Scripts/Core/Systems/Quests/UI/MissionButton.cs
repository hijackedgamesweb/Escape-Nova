using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Quests.UI
{
    public class MissionButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text missionTitleText;
        [SerializeField] private GameObject highlightObject;
    
        private QuestData assignedQuest;
        private MissionsUIController uiController;

        public QuestData AssignedQuest => assignedQuest;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }
        
        public void Setup(QuestData questData, MissionsUIController controller)
        {
            assignedQuest = questData;
            uiController = controller;
            missionTitleText.text = assignedQuest.Title;
            SetSelected(false);
        }

        private void OnButtonClick()
        {
            uiController.DisplayQuestDetails(assignedQuest);
        }
        
        public void SetSelected(bool isSelected)
        {
            if (highlightObject != null)
            {
                highlightObject.SetActive(isSelected);
            }
        }

        public void MarkAskCompleted()
        {
            missionTitleText.text += " (Completed)";
            
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                ColorBlock colors = btn.colors;
                colors.normalColor = Color.green;
                colors.selectedColor = new Color(0, 1, 0, 0.9f);
                colors.pressedColor = new Color(0, 1, 0, 0.9f);
                colors.highlightedColor = new Color(0,1,0,0.9f);
                btn.colors = colors;
            }
        }
        
        public string GetMissionTitle(){return missionTitleText.text;}
    }
}