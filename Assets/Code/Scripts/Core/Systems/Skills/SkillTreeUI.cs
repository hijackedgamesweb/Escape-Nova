using System.Collections.Generic;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Skills;
using Code.Scripts.UI.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Skills
{
    public class SkillTreeUI : BaseUIScreen
    {
        [Header("UI References")]
        [SerializeField] private Transform constellationsContainer;
        [SerializeField] private GameObject constellationAreaPrefab;
        [SerializeField] private GameObject skillNodePrefab;
        [SerializeField] private GameObject nodeModal;
        [SerializeField] private TextMeshProUGUI skillPointsText;
        [SerializeField] private GameObject connectionLinePrefab;

        [Header("Modal References")]
        [SerializeField] private TextMeshProUGUI modalNodeName;
        [SerializeField] private TextMeshProUGUI modalDescription;
        [SerializeField] private TextMeshProUGUI modalCost;
        [SerializeField] private Button modalPurchaseButton;
        [SerializeField] private Button modalCloseButton;

        [Header("UI Settings")]
        [SerializeField] private Vector2 baseNodeSpacing = new Vector2(120f, 120f);
        [SerializeField] private float connectionLineWidth = 3f;

        private SkillTreeManager skillTreeManager;
        private SkillNodeData selectedNode;
        private Dictionary<string, GameObject> constellationAreas = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> nodeUIElements = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> linesContainers = new Dictionary<string, GameObject>();
        private bool isInitialized = false;

        public override void Show(object parameter = null)
        {
            base.Show(parameter);
            Debug.Log("SkillTreeUI: Show called");

            // Forzar inicialización inmediata
            if (!isInitialized)
            {
                Initialize();
            }

            // Si está inicializado, refrescar UI
            if (isInitialized)
            {
                RefreshUI();
            }
            else
            {
                // Si no se pudo inicializar, intentar de nuevo en el próximo frame
                Debug.LogWarning("SkillTreeUI: Initialization failed, will retry next frame");
                Invoke(nameof(DelayedInitialization), 0.01f);
            }
        }

        public override void Hide()
        {
            base.Hide();
            HideModal();
        }

        private void Start()
        {
            Debug.Log("SkillTreeUI: Start method called");
            SetupModalButtons();
            HideModal();
        }

        private void SetupModalButtons()
        {
            if (modalCloseButton != null)
                modalCloseButton.onClick.AddListener(HideModal);

            if (modalPurchaseButton != null)
                modalPurchaseButton.onClick.AddListener(PurchaseSelectedNode);
        }

        private void Initialize()
        {
            if (isInitialized) return;

            Debug.Log("SkillTreeUI: Initializing...");

            try
            {
                skillTreeManager = ServiceLocator.GetService<SkillTreeManager>();

                if (skillTreeManager != null && skillTreeManager.IsInitialized)
                {
                    skillTreeManager.OnSkillPurchased += OnSkillPurchased;
                    skillTreeManager.OnSkillPointsChanged += OnSkillPointsChanged;
                    isInitialized = true;
                    Debug.Log("SkillTreeUI: Initialized successfully with SkillTreeManager");
                }
                else
                {
                    Debug.LogWarning("SkillTreeUI: SkillTreeManager not ready yet");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"SkillTreeUI: Failed to initialize - {e.Message}");
            }
        }

        private void DelayedInitialization()
        {
            if (!isInitialized)
            {
                Debug.Log("SkillTreeUI: Retrying initialization...");
                Initialize();

                if (isInitialized)
                {
                    RefreshUI();
                }
            }
        }

        private void InitializeUI()
        {
            if (!CheckUIReferences())
            {
                Debug.LogError("SkillTreeUI: Missing UI references");
                return;
            }

            if (skillTreeManager == null)
            {
                Debug.LogError("SkillTreeUI: SkillTreeManager is null");
                return;
            }

            ClearUI();

            var constellations = skillTreeManager.GetConstellations();
            if (constellations == null)
            {
                Debug.LogError("SkillTreeUI: Constellations is null");
                return;
            }

            Debug.Log($"SkillTreeUI: Creating UI for {constellations.Count} constellations");

            foreach (var constellation in constellations)
            {
                if (constellation != null)
                {
                    CreateConstellationArea(constellation);
                }
            }

            UpdateSkillPointsDisplay();
        }

        private void ClearUI()
        {
            if (constellationsContainer != null)
            {
                foreach (Transform child in constellationsContainer)
                {
                    Destroy(child.gameObject);
                }
            }
            constellationAreas.Clear();
            nodeUIElements.Clear();
            linesContainers.Clear();
        }

        private bool CheckUIReferences()
        {
            bool allValid = true;

            if (constellationsContainer == null)
            {
                Debug.LogError("SkillTreeUI: ConstellationsContainer is not assigned!");
                allValid = false;
            }

            if (constellationAreaPrefab == null)
            {
                Debug.LogError("SkillTreeUI: ConstellationAreaPrefab is not assigned!");
                allValid = false;
            }

            if (skillNodePrefab == null)
            {
                Debug.LogError("SkillTreeUI: SkillNodePrefab is not assigned!");
                allValid = false;
            }

            return allValid;
        }

        private void CreateConstellationArea(SkillConstellation constellation)
        {
            GameObject area = Instantiate(constellationAreaPrefab, constellationsContainer);
            constellationAreas[constellation.constellationName] = area;

            TextMeshProUGUI headerText = area.GetComponentInChildren<TextMeshProUGUI>();
            if (headerText != null)
            {
                headerText.text = constellation.constellationName;
            }

            // Crear contenedor para líneas
            GameObject linesContainer = new GameObject("LinesContainer");
            linesContainer.transform.SetParent(area.transform);
            linesContainer.transform.SetAsFirstSibling();

            RectTransform linesRect = linesContainer.AddComponent<RectTransform>();
            linesRect.anchorMin = Vector2.zero;
            linesRect.anchorMax = Vector2.one;
            linesRect.offsetMin = Vector2.zero;
            linesRect.offsetMax = Vector2.zero;

            linesContainers[constellation.constellationName] = linesContainer;

            // Crear nodos
            if (constellation.nodes != null)
            {
                foreach (var node in constellation.nodes)
                {
                    if (node != null)
                    {
                        CreateNodeUI(node, area.transform);
                    }
                }

                // Crear conexiones
                CreateConnections(constellation, linesContainer.transform);
            }
        }

        private void CreateNodeUI(SkillNodeData nodeData, Transform parent)
        {
            GameObject nodeUI = Instantiate(skillNodePrefab, parent);

            RectTransform nodeRect = nodeUI.GetComponent<RectTransform>();
            if (nodeRect != null)
            {
                Vector2 position = nodeData.positionInConstellation * baseNodeSpacing;
                nodeRect.anchoredPosition = position;
            }

            SkillNodeUIItem nodeItem = nodeUI.GetComponent<SkillNodeUIItem>();
            if (nodeItem != null)
            {
                nodeItem.Initialize(nodeData, skillTreeManager, this);
                nodeUIElements[nodeData.name] = nodeUI;
            }
        }

        private void CreateConnections(SkillConstellation constellation, Transform parent)
        {
            if (connectionLinePrefab == null) return;

            foreach (var node in constellation.nodes)
            {
                if (node?.prerequisiteNodes == null) continue;

                foreach (var prerequisite in node.prerequisiteNodes)
                {
                    if (prerequisite != null && nodeUIElements.ContainsKey(node.name) && nodeUIElements.ContainsKey(prerequisite.name))
                    {
                        CreateConnectionLine(nodeUIElements[prerequisite.name], nodeUIElements[node.name], parent);
                    }
                }
            }
        }

        private void CreateConnectionLine(GameObject fromNode, GameObject toNode, Transform parent)
        {
            if (fromNode == null || toNode == null || connectionLinePrefab == null) return;

            try
            {
                GameObject line = Instantiate(connectionLinePrefab, parent);
                UILineRenderer lineRenderer = line.GetComponent<UILineRenderer>();

                if (lineRenderer != null)
                {
                    RectTransform fromRect = fromNode.GetComponent<RectTransform>();
                    RectTransform toRect = toNode.GetComponent<RectTransform>();

                    if (fromRect != null && toRect != null)
                    {
                        Vector2 fromPos = fromRect.anchoredPosition;
                        Vector2 toPos = toRect.anchoredPosition;

                        lineRenderer.Points = new Vector2[] { fromPos, toPos };
                        lineRenderer.LineWidth = connectionLineWidth;
                        lineRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                        line.transform.SetAsFirstSibling();
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating connection line: {e.Message}");
            }
        }

        public void ShowNodeModal(SkillNodeData nodeData)
        {
            if (!isInitialized || nodeData == null) return;

            selectedNode = nodeData;

            if (modalNodeName != null) modalNodeName.text = nodeData.nodeName;
            if (modalDescription != null) modalDescription.text = nodeData.description;
            if (modalCost != null) modalCost.text = $"Coste: {nodeData.skillPointCost} puntos";

            if (modalPurchaseButton != null)
            {
                bool canPurchase = skillTreeManager.CanPurchaseSkill(nodeData);
                modalPurchaseButton.interactable = canPurchase;
            }

            if (nodeModal != null)
            {
                nodeModal.SetActive(true);
                nodeModal.transform.SetAsLastSibling();
            }
        }

        public void HideModal()
        {
            if (nodeModal != null) nodeModal.SetActive(false);
            selectedNode = null;
        }

        public void ForceCloseModal()
        {
            HideModal();
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
            if (isInitialized && gameObject.activeInHierarchy)
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
            CancelInvoke(nameof(DelayedInitialization));
        }

        [ContextMenu("Debug UI State")]
        public void DebugUIState()
        {
            Debug.Log($"=== SKILL TREE UI DEBUG ===");
            Debug.Log($"Initialized: {isInitialized}");
            Debug.Log($"SkillTreeManager: {skillTreeManager != null}");
            Debug.Log($"SkillTreeManager IsInitialized: {skillTreeManager?.IsInitialized}");
            Debug.Log($"Active in hierarchy: {gameObject.activeInHierarchy}");
        }
    }
}