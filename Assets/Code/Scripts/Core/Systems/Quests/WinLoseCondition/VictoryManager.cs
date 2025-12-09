using UnityEngine;
using UnityEngine.SceneManagement; // 1. Importante: Necesario para cambiar de escena
using System.Collections;         // 2. Importante: Necesario para Corrutinas
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Quests;
using Code.Scripts.Patterns.ServiceLocator;

public class VictoryManager : MonoBehaviour
{
    [Header("Misión de Victoria")]
    [SerializeField] private string victoryQuestId = "escape";

    [Header("Transición de Victoria")]
    [SerializeField] private string nombreEscenaCreditos = "CreditsScene";
    [SerializeField] private CanvasGroup grupoCanvasFadeOut; 
    [SerializeField] private float duracionFadeOut = 1.5f;

    private QuestManager _questManager;
    private bool _isVictorious = false;

    void Start()
    {
        if (grupoCanvasFadeOut != null)
        {
            grupoCanvasFadeOut.alpha = 0f;
            grupoCanvasFadeOut.gameObject.SetActive(false);
        }
        else
        {
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
        if (completedQuest.questData.QuestId == victoryQuestId && !_isVictorious)
        {
            _isVictorious = true;
            StartCoroutine(SecuenciaVictoria());
        }
    }
    
    private IEnumerator SecuenciaVictoria()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.sfxSource.Stop();
        }
        else
        {
        }
        if (grupoCanvasFadeOut != null)
        {
            grupoCanvasFadeOut.gameObject.SetActive(true);
            float timer = 0f;
            while (timer < duracionFadeOut)
            {
                timer += Time.deltaTime;
                grupoCanvasFadeOut.alpha = Mathf.Clamp01(timer / duracionFadeOut);
                yield return null;
            }
            grupoCanvasFadeOut.alpha = 1f;
        }
        else
        {
            yield return new WaitForSeconds(duracionFadeOut);
        }
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(nombreEscenaCreditos);
    }
}