using UnityEngine;
using UnityEngine.UI;
using Code.Scripts.Core.Systems.Skills;
using TMPro;

namespace Code.Scripts.UI.Skills
{
    public class SkillNodeUIItem : MonoBehaviour
    {
        [SerializeField] private Button nodeButton;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI nodeNameText; // Agregado para mostrar nombre

        [Header("Colors")]
        [SerializeField] private Color purchasedColor = Color.gray;
        [SerializeField] private Color unlockedColor = Color.green;
        [SerializeField] private Color lockedColor = Color.red;

        private SkillNodeData nodeData;
        private SkillTreeManager skillTreeManager;
        private SkillTreeUI skillTreeUI;

        public void Initialize(SkillNodeData data, SkillTreeManager manager, SkillTreeUI ui)
        {
            nodeData = data;
            skillTreeManager = manager;
            skillTreeUI = ui;
            skillTreeUI = ui;

            // Configurar nombre del nodo
            if (nodeNameText != null && nodeData != null)
            {
                nodeNameText.text = nodeData.nodeName;
            }

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
                    // Nodo comprado: color verde, pero SIEMPRE interactuable para mostrar info
                    backgroundImage.color = purchasedColor;
                    nodeButton.interactable = true; // IMPORTANTE: Siempre interactuable
                }
                else if (skillTreeManager.IsSkillUnlocked(nodeData))
                {
                    // Nodo desbloqueado pero no comprado
                    backgroundImage.color = unlockedColor;
                    nodeButton.interactable = true;
                }
                else
                {
                    // Nodo bloqueado
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