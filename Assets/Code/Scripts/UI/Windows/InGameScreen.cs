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
                UIManager.Instance.ShowScreen<ActionPanelScreen>("Astrarium"));
            diplomacyBtn.onClick.AddListener(() => 
                UIManager.Instance.ShowScreen<ActionPanelScreen>("Diplomacy"));
            skillTreeBtn.onClick.AddListener(() => 
                UIManager.Instance.ShowScreen<ActionPanelScreen>("SkillTree"));
            missionsBtn.onClick.AddListener(() => 
                UIManager.Instance.ShowScreen<ActionPanelScreen>("Missions"));
            storageBtn.onClick.AddListener(() => 
                UIManager.Instance.ShowScreen<ActionPanelScreen>("Storage"));
            constructionBtn.onClick.AddListener(() => 
                UIManager.Instance.ShowScreen<PerfectViewScreen>());
            researchBtn.onClick.AddListener(() => 
                UIManager.Instance.ShowScreen<ActionPanelScreen>("Research"));
        }
    }
}