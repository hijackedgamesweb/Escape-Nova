using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public class OptionsScreen : BaseUIScreen
    {
        //Variables
        
        [SerializeField] public Button returnBtn;
        
        
        //Metodos
        
        //AUDIO
        public void OnMasterVolumeSliderValueChanged(Slider sldr)
        {
            AudioManager.Instance.SetMasterVolume(sldr.value);
        }
        
        public void OnMusicVolumeSliderValueChanged(Slider sldr)
        {
            AudioManager.Instance.SetMusicVolume(sldr.value);
        }
        
        public void OnSfxVolumeSliderValueChanged(Slider sldr)
        {
            AudioManager.Instance.SetSFXVolume(sldr.value);
        }
        
        //VIDEO
        
        public void FullscreenTogglePressed()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}