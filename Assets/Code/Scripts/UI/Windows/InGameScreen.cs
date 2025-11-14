using System;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Construction;
using UnityEngine;
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
                ButtonPressed("Astrarium"));
            diplomacyBtn.onClick.AddListener(() => 
                ButtonPressed("Diplomacy"));
            skillTreeBtn.onClick.AddListener(() => 
                ButtonPressed("SkilTree"));
            missionsBtn.onClick.AddListener(() => 
                ButtonPressed("Missions"));
            storageBtn.onClick.AddListener(() => 
                ButtonPressed("Storage"));
            constructionBtn.onClick.AddListener(() => 
                UIManager.Instance.ShowScreen<PerfectViewScreen>());
            researchBtn.onClick.AddListener(() => 
                ButtonPressed("Research"));
        }

        private void ButtonPressed(string uiName)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
            UIManager.Instance.ShowScreen<ActionPanelScreen>(uiName);
        }
    }
}