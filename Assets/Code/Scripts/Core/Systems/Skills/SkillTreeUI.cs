using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems.Skills;

namespace Code.Scripts.UI.Skills
{
    public class SkillTreeUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform constellationsContainer;
        [SerializeField] private GameObject constellationColumnPrefab;
        [SerializeField] private GameObject skillNodePrefab;
        [SerializeField] private GameObject nodeModal;
        [SerializeField] private TextMeshProUGUI skillPointsText;

        [Header("Modal References")]
        [SerializeField] private TextMeshProUGUI modalNodeName;
        [SerializeField] private TextMeshProUGUI modalDescription;
        [SerializeField] private TextMeshProUGUI modalCost;
        [SerializeField] private Button modalPurchaseButton;
        [SerializeField] private Button modalCloseButton;

        private SkillTreeManager skillTreeManager;
        private SkillNodeData selectedNode;
        private Dictionary<string, GameObject> constellationColumns = new Dictionary<string, GameObject>();
        private bool isInitialized = false;

        private void Start()
        {
            StartCoroutine(InitializeWhenReady());

            if (modalCloseButton != null)
                modalCloseButton.onClick.AddListener(HideModal);
            else
                Debug.LogError("SkillTreeUI: ModalCloseButton is not assigned!");

            if (modalPurchaseButton != null)
                modalPurchaseButton.onClick.AddListener(PurchaseSelectedNode);
            else
                Debug.LogError("SkillTreeUI: ModalPurchaseButton is not assigned!");

            HideModal();
        }

        private System.Collections.IEnumerator InitializeWhenReady()
        {
            Debug.Log("SkillTreeUI: Starting initialization...");

            // Esperar a que SkillTreeManager esté disponible
            int attempts = 0;
            int maxAttempts = 50;

            while (skillTreeManager == null && attempts < maxAttempts)
            {
                try
                {
                    skillTreeManager = ServiceLocator.GetService<SkillTreeManager>();
                }
                catch (System.Exception e)
                {
                    Debug.Log($"SkillTreeUI: Attempt {attempts + 1} - SkillTreeManager not ready: {e.Message}");
                }

                if (skillTreeManager == null)
                {
                    attempts++;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            if (skillTreeManager == null)
            {
                Debug.LogError("SkillTreeUI: Failed to get SkillTreeManager after " + maxAttempts + " attempts");
                yield break;
            }

            Debug.Log("SkillTreeUI: SkillTreeManager acquired");

            // Suscribirse a eventos
            skillTreeManager.OnSkillPurchased += OnSkillPurchased;
            skillTreeManager.OnSkillPointsChanged += OnSkillPointsChanged;

            InitializeUI();
            isInitialized = true;

            Debug.Log("SkillTreeUI: Initialization completed");
        }

        private void InitializeUI()
        {
            if (!CheckUIReferences())
            {
                Debug.LogError("SkillTreeUI: Missing required UI references!");
                return;
            }

            if (skillTreeManager == null)
            {
                Debug.LogError("SkillTreeUI: SkillTreeManager is null!");
                return;
            }

            // Limpiar contenedor
            if (constellationsContainer != null)
            {
                foreach (Transform child in constellationsContainer)
                {
                    Destroy(child.gameObject);
                }
            }
            constellationColumns.Clear();

            // Crear columnas para cada constelación
            var constellations = skillTreeManager.GetConstellations();
            if (constellations == null)
            {
                Debug.LogError("SkillTreeUI: Constellations list is null!");
                return;
            }

            Debug.Log($"SkillTreeUI: Creating UI for {constellations.Count} constellations");

            foreach (var constellation in constellations)
            {
                if (constellation == null)
                {
                    Debug.LogError("SkillTreeUI: Found null constellation!");
                    continue;
                }

                CreateConstellationColumn(constellation);
            }

            UpdateSkillPointsDisplay();
        }

        private bool CheckUIReferences()
        {
            bool allValid = true;

            if (constellationsContainer == null)
            {
                Debug.LogError("SkillTreeUI: ConstellationsContainer is not assigned!");
                allValid = false;
            }

            if (constellationColumnPrefab == null)
            {
                Debug.LogError("SkillTreeUI: ConstellationColumnPrefab is not assigned!");
                allValid = false;
            }

            if (skillNodePrefab == null)
            {
                Debug.LogError("SkillTreeUI: SkillNodePrefab is not assigned!");
                allValid = false;
            }

            if (nodeModal == null)
            {
                Debug.LogError("SkillTreeUI: NodeModal is not assigned!");
                allValid = false;
            }

            if (skillPointsText == null)
            {
                Debug.LogError("SkillTreeUI: SkillPointsText is not assigned!");
                allValid = false;
            }

            return allValid;
        }

        private void CreateConstellationColumn(SkillConstellation constellation)
        {
            GameObject column = Instantiate(constellationColumnPrefab, constellationsContainer);
            constellationColumns[constellation.constellationName] = column;

            // Configurar el header de la constelación
            TextMeshProUGUI headerText = column.GetComponentInChildren<TextMeshProUGUI>();
            if (headerText != null)
            {
                headerText.text = constellation.constellationName;
            }
            else
            {
                Debug.LogWarning($"SkillTreeUI: Could not find header text for constellation {constellation.constellationName}");
            }

            // Crear nodos en la columna
            if (constellation.nodes != null)
            {
                foreach (var node in constellation.nodes)
                {
                    if (node != null)
                    {
                        CreateNodeUI(node, column.transform);
                    }
                }
            }
        }

        private void CreateNodeUI(SkillNodeData nodeData, Transform parent)
        {
            GameObject nodeUI = Instantiate(skillNodePrefab, parent);
            SkillNodeUIItem nodeItem = nodeUI.GetComponent<SkillNodeUIItem>();
            if (nodeItem != null)
            {
                nodeItem.Initialize(nodeData, skillTreeManager, this);
            }
            else
            {
                Debug.LogError("SkillTreeUI: SkillNodePrefab is missing SkillNodeUIItem component!");
            }
        }

        public void ShowNodeModal(SkillNodeData nodeData)
        {
            if (!isInitialized || nodeData == null) return;

            selectedNode = nodeData;

            if (modalNodeName != null)
                modalNodeName.text = nodeData.nodeName;
            if (modalDescription != null)
                modalDescription.text = nodeData.description;
            if (modalCost != null)
                modalCost.text = $"Coste: {nodeData.skillPointCost} puntos";

            if (modalPurchaseButton != null)
                modalPurchaseButton.interactable = skillTreeManager.CanPurchaseSkill(nodeData);

            if (nodeModal != null)
                nodeModal.SetActive(true);
        }

        private void HideModal()
        {
            if (nodeModal != null)
                nodeModal.SetActive(false);
            selectedNode = null;
        }

        private void PurchaseSelectedNode()
        {
            if (selectedNode != null && skillTreeManager != null)
            {
                skillTreeManager.PurchaseSkill(selectedNode);
                HideModal();
            }
        }

        private void OnSkillPurchased(SkillNodeData nodeData)
        {
            RefreshUI();
        }

        private void OnSkillPointsChanged(int points)
        {
            UpdateSkillPointsDisplay();
        }

        private void UpdateSkillPointsDisplay()
        {
            if (skillPointsText != null && skillTreeManager != null)
            {
                skillPointsText.text = $"Puntos de Habilidad: {skillTreeManager.GetAvailableSkillPoints()}";
            }
        }

        public void RefreshUI()
        {
            if (isInitialized)
            {
                InitializeUI();
            }
        }

        private void OnDestroy()
        {
            if (skillTreeManager != null)
            {
                skillTreeManager.OnSkillPurchased -= OnSkillPurchased;
                skillTreeManager.OnSkillPointsChanged -= OnSkillPointsChanged;
            }
        }
    }
}