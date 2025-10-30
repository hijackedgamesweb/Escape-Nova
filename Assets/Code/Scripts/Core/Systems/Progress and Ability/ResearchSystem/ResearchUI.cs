using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Research
{
        // ResearchUI.cs
    
    
    public class ResearchUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform researchPanel;
        [SerializeField] private GameObject researchItemPrefab;
        [SerializeField] private TextMeshProUGUI currentResearchText;
        [SerializeField] private Slider researchProgressSlider;
        [SerializeField] private TextMeshProUGUI resourcesText;
    
        private ResearchSystem _researchSystem;
        private Dictionary<string, GameObject> _researchUIItems = new Dictionary<string, GameObject>();
    
        private void Start()
        {
            _researchSystem = ServiceLocator.GetService<ResearchSystem>();
            
            // Suscripcion de eventos
            _researchSystem.OnResearchStarted += OnResearchStarted;
            _researchSystem.OnResearchProgress += OnResearchProgress;
            _researchSystem.OnResearchCompleted += OnResearchCompleted;
            _researchSystem.OnResearchUnlocked += OnResearchUnlocked;
            
            InitializeUI();
            UpdateResourcesUI();
        }
    
        private void InitializeUI()
        {
            Debug.Log("INICIALIZANDO UI - Limpiando items previos");
    
            if (researchPanel == null)
            {
                Debug.LogError("ResearchPanel es NULL!");
                return;
            }
    
            // Destruir todos los hijos del panel
            foreach (Transform child in researchPanel)
            {
                Debug.Log($"Destruyendo hijo: {child.name}");
                Destroy(child.gameObject);
            }
    
            _researchUIItems.Clear();
            Debug.Log($"Panel limpio. Hijos restantes: {researchPanel.childCount}");

            // VERIFICAR SISTEMA
            if (_researchSystem == null)
            {
                Debug.LogError("ResearchSystem es NULL!");
                return;
            }

            var allResearch = _researchSystem.GetAllResearchStatus();
            Debug.Log($"Investigaciones a crear: {allResearch.Count}");

            // CREAR NUEVOS ITEMS
            foreach (var researchStatus in allResearch)
            {
                CreateResearchUIItem(researchStatus.Key);
            }
    
            UpdateUILayout();
            UpdateCurrentResearchDisplay();
            
            Debug.Log($"UI Inicializada. Total items creados: {researchPanel.childCount}");
        }
        
        private void UpdateUILayout()
        {
            // Forzar actualización del layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(researchPanel as RectTransform);
    
            // Si tiene scroll, ajustarlo
            ScrollRect scrollRect = researchPanel.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.horizontalNormalizedPosition = 0;
            }
        }
    
        private void CreateResearchUIItem(string researchId)
        {
            var researchNode = _researchSystem.GetResearch(researchId);
            if (researchNode == null) return;
    
            GameObject uiItem = Instantiate(researchItemPrefab, researchPanel);
            var researchItem = uiItem.GetComponent<ResearchUIItem>();
            
            if (researchItem != null)
            {
                researchItem.Initialize(researchNode, _researchSystem, this);
            }
            
            _researchUIItems[researchId] = uiItem;
        }
    
        private void OnResearchStarted(string researchId)
        {
            Debug.Log($"UI: Investigación iniciada - {researchId}");
            UpdateCurrentResearchDisplay();
            UpdateAllResearchButtons();
        }
    
        private void OnResearchProgress(string researchId, float progress)
        {
            researchProgressSlider.value = progress;
            currentResearchText.text = $"Investigando: {_researchSystem.GetResearch(researchId)?.displayName} - {(progress * 100):F1}%";
    
            // AÑADIDO: Actualizar el botón específico que está en progreso
            if (_researchUIItems.ContainsKey(researchId))
            {
                var researchItem = _researchUIItems[researchId].GetComponent<ResearchUIItem>();
                if (researchItem != null)
                {
                    researchItem.UpdateButtonState();
                }
            }
        }
    
        private void OnResearchCompleted(string researchId)
        {
            Debug.Log($"UI: Investigación completada - {researchId}");
            UpdateCurrentResearchDisplay();
            UpdateAllResearchButtons();
            UpdateResourcesUI();
        }
    
        private void OnResearchUnlocked(string researchId)
        {
            Debug.Log($"UI: Nueva investigación disponible - {researchId}");
            UpdateAllResearchButtons();
        }
    
        private void UpdateCurrentResearchDisplay()
        {
            // Encontrar investigación en progreso
            foreach (var research in _researchSystem.GetAllResearchStatus())
            {
                if (research.Value == ResearchStatus.InProgress)
                {
                    var researchNode = _researchSystem.GetResearch(research.Key);
                    currentResearchText.text = $"Investigando: {researchNode.displayName}";
                    researchProgressSlider.gameObject.SetActive(true);
                    return;
                }
            }
            
            // No hay investigación en progreso
            currentResearchText.text = "No hay investigación en curso";
            researchProgressSlider.gameObject.SetActive(false);
        }
    
        private void UpdateAllResearchButtons()
        {
            foreach (var uiItem in _researchUIItems)
            {
                var researchItem = uiItem.Value.GetComponent<ResearchUIItem>();
                if (researchItem != null)
                {
                    researchItem.UpdateButtonState();
                }
            }
        }
    
        public void UpdateResourcesUI()
        {
            if (resourcesText == null)
            {
                Debug.LogError("ResourcesText no asignado en ResearchUI!");
                return;
            }

            StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
            if (storage == null)
            {
                Debug.LogError("StorageSystem no encontrado!");
                return;
            }

            var resources = storage.GetAllResources();
    
            string resourcesInfo = "Recursos: ";
            foreach (var resource in resources)
            {
                resourcesInfo += $"{resource.Key}: {resource.Value} ";
            }
    
            resourcesText.text = resourcesInfo;
            Debug.Log($"UI Actualizada: {resourcesInfo}");
        }
    }
}