using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Research
{
    // ResearchUIItem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Code.Scripts.Core.Systems.Research;

public class ResearchUIItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button researchButton;
    [SerializeField] private Image backgroundImage;

    [Header("Colors")]
    [SerializeField] private Color availableColor = Color.green;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color inProgressColor = Color.yellow;
    [SerializeField] private Color completedColor = Color.blue;

    private ResearchNode _researchNode;
    private ResearchSystem _researchSystem;
    private ResearchUI _researchUI;

    public void Initialize(ResearchNode researchNode, ResearchSystem researchSystem, ResearchUI researchUI)
    {
        Debug.Log($"=== INICIALIZANDO RESEARCH UI ITEM ===");
        Debug.Log($"ResearchNode: {researchNode?.researchId ?? "NULL"}");
        Debug.Log($"ResearchSystem: {researchSystem != null}");
        Debug.Log($"ResearchUI: {researchUI != null}");

        _researchNode = researchNode;
        _researchSystem = researchSystem;
        _researchUI = researchUI;

        // Configurar el botón por código para asegurar
        if (researchButton != null)
        {
            researchButton.onClick.RemoveAllListeners();
            researchButton.onClick.AddListener(OnResearchButtonClicked);
            Debug.Log("Botón configurado correctamente");
        }
        else
        {
            Debug.LogError("ResearchButton no asignado en el inspector!");
        }

        // Verificar que los textos están asignados
        if (nameText == null) Debug.LogError("nameText no asignado!");
        if (descriptionText == null) Debug.LogError("descriptionText no asignado!");
        if (costText == null) Debug.LogError("costText no asignado!");
        if (timeText == null) Debug.LogError("timeText no asignado!");
        if (backgroundImage == null) Debug.LogError("backgroundImage no asignado!");

        UpdateUI();
    
        Debug.Log($"ResearchUIItem inicializado completamente para: {researchNode?.researchId}");
    }

    private void UpdateUI()
    {
        nameText.text = _researchNode.displayName;
        descriptionText.text = _researchNode.description;
        timeText.text = $"Tiempo: {_researchNode.researchTimeInSeconds}s";

        // Mostrar costos
        string costInfo = "Coste: ";
        foreach (var cost in _researchNode.resourceCosts)
        {
            if (cost.useInventoryItem)
            {
                costInfo += $"{cost.itemName}: {cost.amount} ";
            }
            else
            {
                costInfo += $"{cost.resourceType}: {cost.amount} ";
            }
        }
        costText.text = costInfo;

        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        if (_researchSystem == null) return;
        
        var status = _researchSystem.GetResearchStatus(_researchNode.researchId);
        bool isAnyResearchInProgress = _researchSystem.IsAnyResearchInProgress();
        string currentResearchId = _researchSystem.GetCurrentResearchId();
        
        bool hasEnoughResources = CheckResources();
        
        switch (status)
        {
            case ResearchStatus.Available:
                // DESHABILITAR si hay otra investigación en curso O no tiene recursos
                bool canResearch = !isAnyResearchInProgress && hasEnoughResources;
                
                researchButton.interactable = canResearch;
                
                if (isAnyResearchInProgress)
                {
                    researchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Investigación en curso";
                    backgroundImage.color = Color.gray;
                }
                else if (!hasEnoughResources)
                {
                    researchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Recursos insuficientes";
                    backgroundImage.color = lockedColor; // Gris por falta de recursos
                }
                else
                {
                    researchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Investigar";
                    backgroundImage.color = availableColor; // Verde - puede investigar
                }
                break;
                
            case ResearchStatus.Locked:
                researchButton.interactable = false;
                researchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Bloqueado";
                backgroundImage.color = lockedColor;
                break;
                
            case ResearchStatus.InProgress:
                researchButton.interactable = false;
                float progress = _researchSystem.GetResearchProgress(_researchNode.researchId) * 100;
                researchButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                    $"Investigando... {progress:F1}%";
                backgroundImage.color = inProgressColor;
                break;
                
            case ResearchStatus.Completed:
                researchButton.interactable = false;
                researchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Completado";
                backgroundImage.color = completedColor;
                break;
        }
    }
    
    private bool CheckResources()
    {
        if (_researchSystem == null || _researchNode == null) return false;
        
        StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
        if (storage == null) return false;
        
        foreach (var cost in _researchNode.resourceCosts)
        {
            if (cost.useInventoryItem)
            {
                if (!storage.HasInventoryItem(cost.itemName, cost.amount))
                    return false;
            }
            else
            {
                if (!storage.HasResource(cost.resourceType, cost.amount))
                    return false;
            }
        }
        
        return true;
    }

    // Llamado por el botón en la UI
    public void OnResearchButtonClicked()
    {
        Debug.Log("=== ON RESEARCH BUTTON CLICKED ===");
    
        // Verificar todas las referencias críticas
        if (_researchSystem == null)
        {
            Debug.LogError("ResearchSystem es null! No se pudo obtener del ServiceLocator?");
            _researchSystem = ServiceLocator.GetService<ResearchSystem>();
            if (_researchSystem == null)
            {
                Debug.LogError("Todavía no se puede obtener ResearchSystem del ServiceLocator");
                return;
            }
        }
    
        if (_researchNode == null)
        {
            Debug.LogError("ResearchNode es null! No se inicializó correctamente?");
            return;
        }
    
        if (_researchUI == null)
        {
            Debug.LogError("ResearchUI es null!");
            return;
        }
    
        Debug.Log($"Intentando iniciar investigación: {_researchNode.researchId}");
    
        bool canResearch = _researchSystem.CanStartResearch(_researchNode.researchId);
        Debug.Log($"¿Puede investigar? {canResearch}");
    
        if (canResearch)
        {
            bool started = _researchSystem.StartResearch(_researchNode.researchId);
            Debug.Log($"¿Investigación iniciada? {started}");
        
            if (_researchUI != null)
            {
                _researchUI.UpdateResourcesUI();
            }
        }
        else
        {
            Debug.Log($"No se puede iniciar la investigación. Estado: {_researchSystem.GetResearchStatus(_researchNode.researchId)}");
        }
    }
}
}