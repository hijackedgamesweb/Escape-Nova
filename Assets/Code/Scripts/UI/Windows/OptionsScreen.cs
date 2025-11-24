using Code.Scripts.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public class OptionsScreen : BaseUIScreen
    {
        [SerializeField] public Button returnBtn;
        private string _previousScreen;

        private void Awake()
        {
            returnBtn.onClick.AddListener(OnReturnPressed);
        }
        public override void Show(object parameter = null)
        {
            base.Show(parameter);

            if (parameter is string origin)
            {
                _previousScreen = origin;
            }
            else
            {
                _previousScreen = "MainMenu";
            }
        }

        private void OnReturnPressed()
        {
            AudioManager.Instance.PlaySFX("ButtonClick");

            if (_previousScreen == "PauseMenu")
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
                UIManager.Instance.ShowOverlay<PauseMenuScreen>();
            }
            else 
            {
                UIManager.Instance.ShowScreen<MainMenuScreen>();
            }
        }
        public void OnMasterVolumeSliderValueChanged(Slider sldr) => AudioManager.Instance.SetMasterVolume(sldr.value);
        public void OnMusicVolumeSliderValueChanged(Slider sldr) => AudioManager.Instance.SetMusicVolume(sldr.value);
        public void OnSfxVolumeSliderValueChanged(Slider sldr) => AudioManager.Instance.SetSFXVolume(sldr.value);
        public void FullscreenTogglePressed() => Screen.fullScreen = !Screen.fullScreen;
    }
}