using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Skills;
using Code.Scripts.UI.Windows;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] private float connectionLineWidth = 1f;

        private SkillTreeManager skillTreeManager;
        private SkillNodeData selectedNode;
        private Dictionary<string, GameObject> constellationAreas = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> nodeUIElements = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> linesContainers = new Dictionary<string, GameObject>();
        private bool isInitialized = false;

        // Evento para notificar cuando esté listo
        public System.Action OnInitialized;

        public override void Show(object parameter = null)
        {
            base.Show(parameter);
           // Debug.Log($"SkillTreeUI: Show called - Active: {gameObject.activeInHierarchy}");

            // Inicialización inmediata y agresiva
            if (!isInitialized)
            {
                StartCoroutine(InitializeRoutine());
            }
            else
            {
                RefreshUI();
            }
        }

        public override void Hide()
        {
            base.Hide();
            HideModal();
        }

        private void Start()
        {
//            Debug.Log("SkillTreeUI: Start method called");
            ForceSetupModalButtons();
            HideModal();
        }
        
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
            }
        }

        private void ForceSetupModalButtons()
        {
          //  Debug.Log("SkillTreeUI: Force setting up modal buttons");

            // Buscar referencias si no están asignadas
            if (modalCloseButton == null && nodeModal != null)
            {
                modalCloseButton = nodeModal.GetComponentInChildren<Button>();
               // Debug.Log($"SkillTreeUI: Searched for close button - found: {modalCloseButton != null}");
            }

            if (modalPurchaseButton == null && nodeModal != null)
            {
                var buttons = nodeModal.GetComponentsInChildren<Button>();
                foreach (var button in buttons)
                {
                    if (button != modalCloseButton)
                    {
                        modalPurchaseButton = button;
                        break;
                    }
                }
             //   Debug.Log($"SkillTreeUI: Searched for purchase button - found: {modalPurchaseButton != null}");
            }

            // Configurar close button
            if (modalCloseButton != null)
            {
                modalCloseButton.onClick.RemoveAllListeners();
                modalCloseButton.onClick.AddListener(OnCloseButtonClicked);
              // Debug.Log("SkillTreeUI: Close button configured successfully");
            }
            else
            {
                Debug.LogError("SkillTreeUI: ModalCloseButton is null after search!");
            }

            // Configurar purchase button
            if (modalPurchaseButton != null)
            {
                modalPurchaseButton.onClick.RemoveAllListeners();
                modalPurchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
              //  Debug.Log("SkillTreeUI: Purchase button configured successfully");
            }
            else
            {
                Debug.LogError("SkillTreeUI: ModalPurchaseButton is null after search!");
            }
        }

        private void OnCloseButtonClicked()
        {
           // Debug.Log("SkillTreeUI: Close button clicked!");
            HideModal();
        }

        private void OnPurchaseButtonClicked()
        {
          //  Debug.Log("SkillTreeUI: Purchase button clicked!");
            PurchaseSelectedNode();
        }

        private IEnumerator InitializeRoutine()
        {
            if (isInitialized) yield break;

           // Debug.Log("SkillTreeUI: Starting initialization routine...");

            int maxAttempts = 10;
            for (int i = 0; i < maxAttempts; i++)
            {
                if (TryInitialize())
                {
                    isInitialized = true;
                  //  Debug.Log("SkillTreeUI: Initialization successful!");
                    OnInitialized?.Invoke();
                    RefreshUI();
                    yield break;
                }

                //Debug.Log($"SkillTreeUI: Initialization attempt {i + 1}/{maxAttempts} failed");
                yield return new WaitForSeconds(0.1f);
            }

            Debug.LogError("SkillTreeUI: Failed to initialize after all attempts");
        }

        private bool TryInitialize()
        {
            try
            {
                skillTreeManager = ServiceLocator.GetService<SkillTreeManager>();

                if (skillTreeManager != null && skillTreeManager.IsInitialized)
                {
                    skillTreeManager.OnSkillPurchased += OnSkillPurchased;
                    skillTreeManager.OnSkillPointsChanged += OnSkillPointsChanged;
                    return true;
                }

                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"SkillTreeUI: Initialize attempt failed: {e.Message}");
                return false;
            }
        }

        // En InitializeUI, modificar el bucle:
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

            int constellationIndex = 0;
            foreach (var constellation in constellations)
            {
                if (constellation != null)
                {
                    CreateConstellationArea(constellation, constellationIndex);
                    constellationIndex++;
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
                    if (child != null && child.gameObject != null)
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

        [Header("Constellation Colors")]
        [SerializeField]
        private Color[] constellationColors = new Color[]
        {
            new Color(0.4f, 0.7f, 1f),    // Azul
            new Color(1f, 0.4f, 0.7f),    // Rosa
            new Color(0.7f, 1f, 0.4f),    // Verde
            new Color(1f, 0.7f, 0.4f),    // Naranja
            new Color(0.7f, 0.4f, 1f)     // Púrpura
        };

        // Modificar el método CreateConstellationArea:
        private void CreateConstellationArea(SkillConstellation constellation, int constellationIndex)
        {
            if (constellationAreaPrefab == null || constellationsContainer == null) return;

            GameObject area = Instantiate(constellationAreaPrefab, constellationsContainer);
            constellationAreas[constellation.constellationName] = area;

            // Asignar color a la constelación
            Color constellationColor = constellationColors[constellationIndex % constellationColors.Length];

            // Aplicar color al título
            TextMeshProUGUI headerText = area.GetComponentInChildren<TextMeshProUGUI>();
            if (headerText != null)
            {
                headerText.text = constellation.constellationName;
                headerText.color = constellationColor;
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

            // Crear nodos con color de constelación
            if (constellation.nodes != null)
            {
                foreach (var node in constellation.nodes)
                {
                    if (node != null)
                    {
                        CreateNodeUI(node, area.transform, constellationColor);
                    }
                }

                // Crear conexiones con color de constelación
                CreateConnections(constellation, linesContainer.transform, constellationColor);
            }
        }



        private void CreateNodeUI(SkillNodeData nodeData, Transform parent, Color constellationColor)
        {
            if (skillNodePrefab == null) return;

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
                // Pasar el color de la constelación al nodo
                nodeItem.SetConstellationColor(constellationColor);
                nodeItem.Initialize(nodeData, skillTreeManager, this);
                nodeUIElements[nodeData.name] = nodeUI;
            }
        }

        private void CreateConnections(SkillConstellation constellation, Transform parent, Color lineColor)
        {
            if (connectionLinePrefab == null) return;

            foreach (var node in constellation.nodes)
            {
                if (node?.prerequisiteNodes == null) continue;

                foreach (var prerequisite in node.prerequisiteNodes)
                {
                    if (prerequisite != null && nodeUIElements.ContainsKey(node.name) && nodeUIElements.ContainsKey(prerequisite.name))
                    {
                        CreateConnectionLine(nodeUIElements[prerequisite.name], nodeUIElements[node.name], parent, lineColor);
                    }
                }
            }
        }

        private void CreateConnectionLine(GameObject fromNode, GameObject toNode, Transform parent, Color lineColor)
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
                        // Obtener posiciones en coordenadas del mundo
                        Vector3 fromWorldPos = fromRect.position;
                        Vector3 toWorldPos = toRect.position;

                        // Convertir a posición local relativa al contenedor de líneas
                        Vector2 fromLocalPos = parent.InverseTransformPoint(fromWorldPos);
                        Vector2 toLocalPos = parent.InverseTransformPoint(toWorldPos);

                        // Establecer los puntos de la línea con color de constelación
                        lineRenderer.Points = new Vector2[] { fromLocalPos, toLocalPos };
                        lineRenderer.LineWidth = connectionLineWidth;
                        lineRenderer.color = new Color(lineColor.r, lineColor.g, lineColor.b, 0.5f);

                        // Forzar actualización del renderizado
                        lineRenderer.SetVerticesDirty();
                        lineRenderer.SetAllDirty();

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

            // Determinar si el nodo ya está comprado
            bool isPurchased = skillTreeManager != null && skillTreeManager.IsSkillPurchased(nodeData);

            if (modalCost != null)
            {
                    modalCost.text = $"Price: {nodeData.skillPointCost} Star Points";
                    modalCost.color = Color.white;
            }

            if (modalPurchaseButton != null)
            {
                // Obtener el texto del botón
                TextMeshProUGUI buttonText = modalPurchaseButton.GetComponentInChildren<TextMeshProUGUI>();

                if (isPurchased)
                {
                    // Si ya está comprado, cambiar texto y deshabilitar
                    if (buttonText != null)
                    {
                        buttonText.text = "PURCHASED";
                    }
                    modalPurchaseButton.interactable = false;
                }
                else
                {
                    // Si no está comprado, verificar si se puede comprar
                    bool canPurchase = skillTreeManager.CanPurchaseSkill(nodeData);
                    modalPurchaseButton.interactable = canPurchase;

                    if (buttonText != null)
                    {
                        buttonText.text = "BUY";
                    }
                }
            }

            if (nodeModal != null)
            {
                nodeModal.SetActive(true);
                nodeModal.transform.SetAsLastSibling();
                // Debug.Log("SkillTreeUI: Modal shown successfully");
            }
            else
            {
                Debug.LogError("SkillTreeUI: NodeModal is null!");
            }
        }

        public void HideModal()
        {
            if (nodeModal != null)
            {
                nodeModal.SetActive(false);
               // Debug.Log("SkillTreeUI: Modal hidden");
            }
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
                if (skillTreeManager.PurchaseSkill(selectedNode))
                {
                    // Detener efectos en el nodo comprado
                    if (nodeUIElements.ContainsKey(selectedNode.name))
                    {
                        SkillNodeUIItem nodeItem = nodeUIElements[selectedNode.name].GetComponent<SkillNodeUIItem>();
                        if (nodeItem != null)
                        {
                            nodeItem.StopAllEffects();
                            nodeItem.PlayPurchaseEffect();
                        }
                    }

                    HideModal();
                }
            }
        }

        private void OnSkillPurchased(SkillNodeData nodeData)
        {
            RefreshUI();
            AudioManager.Instance.PlaySFX("Confirmation");
        }

        private void OnSkillPointsChanged(int points)
        {
            UpdateSkillPointsDisplay();
        }

        private void UpdateSkillPointsDisplay()
        {
            if (skillPointsText != null && skillTreeManager != null)
            {
                skillPointsText.text = $"Star Points: {skillTreeManager.GetAvailableSkillPoints()}";
            }
        }

        public void RefreshUI()
        {
            if (isInitialized && gameObject.activeInHierarchy)
            {
                InitializeUI();
            }
        }

        public bool IsInitialized()
        {
            return isInitialized;
        }

        // Método público para forzar inicialización
        public void ForceInitialize()
        {
            if (!isInitialized)
            {
                StartCoroutine(InitializeRoutine());
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