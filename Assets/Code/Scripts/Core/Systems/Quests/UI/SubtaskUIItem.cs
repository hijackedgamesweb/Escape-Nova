using TMPro;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.UI
{

    public class SubtaskUIItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text subtaskDescriptionText;
        [SerializeField] private GameObject completionIndicator;
    
        private QuestObjective runtimeObjective;
        
        public void Setup(QuestObjective objective)
        {
            runtimeObjective = objective;
            if (subtaskDescriptionText != null)
            {
                subtaskDescriptionText.text = objective.objectiveDescription;
            }
            if (completionIndicator != null)
            {
                completionIndicator.SetActive(runtimeObjective.isCompleted);
            }
        }
        public void UpdateVisuals()
        {
            if (runtimeObjective == null || completionIndicator == null) return;

            if (runtimeObjective.isCompleted != completionIndicator.activeSelf)
            {
                completionIndicator.SetActive(runtimeObjective.isCompleted);
            }
        }
    }
}