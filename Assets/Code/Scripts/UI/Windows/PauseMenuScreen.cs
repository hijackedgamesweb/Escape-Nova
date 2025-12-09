using Code.Scripts.Core.Managers;
using Code.Scripts.Core.SaveLoad;
using Code.Scripts.Core.SceneManagement;
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

        async void OnExitPressed()
        {
            await SaveManager.Instance.SaveAsync();
            Time.timeScale = 1f;
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySFX("ButtonClick");
            AudioManager.Instance.PlaySFX("Spaceship");
            AudioManager.Instance.PlayMusic("MainMenuMusic");
            this.Hide();
            Loader.Load("MainMenu");
        }
    }
}