using UnityEngine;
using UnityEngine.UI;
using Code.Scripts.Core.Systems.Skills;

namespace Code.Scripts.UI.Skills
{
    public class SkillNodeUIItem : MonoBehaviour
    {
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

            if (nodeButton != null)
            {
                nodeButton.onClick.RemoveAllListeners();
                nodeButton.onClick.AddListener(OnNodeClicked);
            }
            else
            {
                Debug.LogError("SkillNodeUIItem: NodeButton is not assigned!");
            }

            UpdateVisualState();
        }

        private void OnNodeClicked()
        {
            if (skillTreeUI != null && nodeData != null)
            {
                skillTreeUI.ShowNodeModal(nodeData);
            }
        }

        public void UpdateVisualState()
        {
            if (nodeData == null || skillTreeManager == null || backgroundImage == null || nodeButton == null)
            {
                Debug.LogWarning("SkillNodeUIItem: Missing references for visual state update");
                return;
            }

            try
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
            catch (System.Exception e)
            {
                Debug.LogError($"Error updating visual state for node {nodeData.nodeName}: {e.Message}");
                backgroundImage.color = lockedColor;
                nodeButton.interactable = false;
            }
        }
    }
}