using System;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Construction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows
{
    public class InGameScreen : BaseUIScreen
    {
        [SerializeField] public Button returnBtn;
        [SerializeField] public Button astrariumBtn;
        [SerializeField] public Button diplomacyBtn;
        [SerializeField] public Button skillTreeBtn;
        [SerializeField] public Button constructionBtn;
        [SerializeField] public Button storageBtn;
        [SerializeField] public Button missionsBtn;
        [SerializeField] public Button researchBtn;

        private void Awake()
        {
            astrariumBtn.onClick.AddListener(() => 
                OnButtonPressed("Astrarium"));
            diplomacyBtn.onClick.AddListener(() => 
                OnButtonPressed("Diplomacy"));
            skillTreeBtn.onClick.AddListener(() => 
                OnButtonPressed("Constellations"));
            missionsBtn.onClick.AddListener(() => 
                OnButtonPressed("Objectives"));
            storageBtn.onClick.AddListener(() => 
                OnButtonPressed("Storage"));
            constructionBtn.onClick.AddListener(() => 
                UIManager.Instance.ShowScreen<PerfectViewScreen>());
            researchBtn.onClick.AddListener(() => 
                OnButtonPressed("Research"));
            returnBtn.onClick.AddListener(() => 
                OpenPauseMenu());
            
            storageBtn.interactable = false;
            researchBtn.interactable = false;
            skillTreeBtn.interactable = false;
            diplomacyBtn.interactable = false;
            SystemEvents.OnInventoryUnlocked += EnableStorageButton;
            SystemEvents.OnResearchUnlocked += EnableResearchButton;
            SystemEvents.OnConstellationsUnlocked += EnableConstellationsButton;
            SystemEvents.OnDiplomacyUnlocked += EnableDiplomacyButton;
        }

        private void EnableDiplomacyButton()
        {
            diplomacyBtn.interactable = true;
        }

        private void Update()
        {
            bool pausePressed = false;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame || 
                    Keyboard.current.tabKey.wasPressedThisFrame)
                {
                    pausePressed = true;
                }
            }
            if (pausePressed)
            {
                OpenPauseMenu();
            }
        }

        private void OpenPauseMenu()
        {
            Time.timeScale = 0f;
            UIManager.Instance.ShowOverlay<PauseMenuScreen>();
        }

        private void OnDestroy()
        {
            SystemEvents.OnInventoryUnlocked -= EnableStorageButton;
            SystemEvents.OnResearchUnlocked -= EnableResearchButton;
            SystemEvents.OnConstellationsUnlocked -= EnableConstellationsButton;
            SystemEvents.OnDiplomacyUnlocked -= EnableDiplomacyButton;
        }
        
        private void OnButtonPressed(String interf)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
            UIManager.Instance.ShowScreen<ActionPanelScreen>(interf);
        }

        public void OnMenuPressed(String interf)
        {
            UIManager.Instance.ShowScreen<OptionsScreen>(interf);
        }
        
        private void EnableStorageButton()
        {
            storageBtn.interactable = true;
        }

        private void EnableResearchButton()
        {
            researchBtn.interactable = true;
        }
        
        private void EnableConstellationsButton()
        {
            skillTreeBtn.interactable = true;
        }
    }
}