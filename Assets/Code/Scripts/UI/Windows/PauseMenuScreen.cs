using Code.Scripts.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public class PauseMenuScreen : BaseUIScreen
    {
        [SerializeField] public Button resumeButton;
        [SerializeField] public Button optionsButton;
        [SerializeField] public Button exitButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(OnResumePressed);
            optionsButton.onClick.AddListener(OnOptionsPressed);
            exitButton.onClick.AddListener(OnExitPressed);
        }

        private void OnResumePressed()
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
            Time.timeScale = 1f;
            this.Hide();
        }

        private void OnOptionsPressed()
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
            UIManager.Instance.ShowScreen<OptionsScreen>("PauseMenu");
            this.Hide();
        }

        private void OnExitPressed()
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
            Time.timeScale = 1f;
            UIManager.Instance.ShowScreen<MainMenuScreen>();
            this.Hide();
        }
    }
}