using UnityEngine;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Quests;
using Code.Scripts.Patterns.ServiceLocator;

public class VictoryManager : MonoBehaviour
{
    [Header("Misión de Victoria")]
    [SerializeField] private string victoryQuestId = "escape";

    [Header("Pantalla de Créditos")]
    [SerializeField] private GameObject creditsPanel;

    private QuestManager _questManager;

    void Start()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }

        _questManager = ServiceLocator.GetService<QuestManager>();
        if (_questManager != null)
        {
            _questManager.OnQuestCompleted += HandleQuestCompleted;
        }
        else
        {
            return;
        }

        _questManager.StartQuest(victoryQuestId);
    }

    private void OnDestroy()
    {
        if (_questManager != null)
        {
            _questManager.OnQuestCompleted -= HandleQuestCompleted;
        }
    }

    private void HandleQuestCompleted(QuestInstance completedQuest)
    {
        if (completedQuest.questData.QuestId == victoryQuestId)
        {
            
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(true);
            }
        }
    }
}