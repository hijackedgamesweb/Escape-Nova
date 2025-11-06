using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.Scripts.Core.Systems.Skills;

namespace Code.Scripts.UI.Skills
{
    public class SkillNodeUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nodeNameText;
        [SerializeField] private Button nodeButton;
        [SerializeField] private Image backgroundImage;

        [Header("Colors")]
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color purchasedColor = Color.green;
        [SerializeField] private Color lockedColor = Color.gray;

        private SkillNodeData nodeData;
        private SkillTreeManager skillTreeManager;
        private SkillTreeUI skillTreeUI;

        public void Initialize(SkillNodeData data, SkillTreeManager manager, SkillTreeUI ui)
        {
            nodeData = data;
            skillTreeManager = manager;
            skillTreeUI = ui;

            nodeNameText.text = data.nodeName;
            nodeButton.onClick.AddListener(OnNodeClicked);
            UpdateVisualState();
        }

        private void OnNodeClicked()
        {
            skillTreeUI.ShowNodeModal(nodeData);
        }

        public void UpdateVisualState()
        {
            if (skillTreeManager.IsSkillPurchased(nodeData))
            {
                backgroundImage.color = purchasedColor;
                nodeButton.interactable = false;
            }
            else if (skillTreeManager.IsSkillUnlocked(nodeData))
            {
                backgroundImage.color = availableColor;
                nodeButton.interactable = true;
            }
            else
            {
                backgroundImage.color = lockedColor;
                nodeButton.interactable = false;
            }
        }
    }
}