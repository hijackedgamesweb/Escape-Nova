using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.Scripts.Core.Systems.Research;
using UnityEngine.EventSystems;

namespace Code.Scripts.UI.Research
{
    public class ResearchUIItem : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject selectionHighlight;

        [Header("Colors (Estado)")]
        [SerializeField] private Color availableColor = Color.green;
        [SerializeField] private Color lockedColor = Color.gray;
        [SerializeField] private Color inProgressColor = Color.yellow;
        [SerializeField] private Color completedColor = Color.blue;

        private ResearchNode _researchNode;
        private ResearchSystem _researchSystem;
        private ResearchPanelUI _researchPanelUI;

        public void Initialize(ResearchNode researchNode, ResearchSystem researchSystem, ResearchPanelUI researchPanel)
        {
            _researchNode = researchNode;
            _researchSystem = researchSystem;
            _researchPanelUI = researchPanel;
            
            nameText.text = _researchNode.displayName;
            iconImage.sprite = _researchNode.icon;
            
            UpdateButtonState();
            SetSelected(false);
        }

        public void UpdateButtonState()
        {
            if (_researchSystem == null || backgroundImage == null) return;
            
            var status = _researchSystem.GetResearchStatus(_researchNode.researchId);
            
            switch (status)
            {
                case ResearchStatus.Available:
                    backgroundImage.color = availableColor;
                    break;
                case ResearchStatus.Locked:
                    backgroundImage.color = lockedColor;
                    break;
                case ResearchStatus.InProgress:
                    backgroundImage.color = inProgressColor;
                    break;
                case ResearchStatus.Completed:
                    backgroundImage.color = completedColor;
                    break;
            }
        }
        
        public void SetSelected(bool isSelected)
        {
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(isSelected);
            }
        }

        public ResearchNode GetNodeData()
        {
            return _researchNode;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _researchPanelUI.OnItemClicked(this);
        }
    }
}