using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Resources;

namespace Code.Scripts.Core.Systems.Research
{
using System.Collections.Generic;
using UnityEngine;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

public class ResearchInitializer : MonoBehaviour
{
    [Header("Research Configuration")]
    [SerializeField] private List<ResearchNode> availableResearchNodes;
    
    [Header("Storage Configuration")]
    [SerializeField] private List<ResourceData> resourceDataList;
    
    [SerializeField] private InventoryData startingInventory;
    
    private ResearchSystem _researchSystem;
    private StorageSystem _storageSystem;

    private void Awake()
    {
        InitializeSystems();
        InitializeSystems();
    }
    
    private void Start()
    {
        if (_researchSystem != null)
        {
            _researchSystem.InitializeDependencies(); 
        }
        SetupTestEnvironment();
        LogInitialState();
    }

    private void InitializeSystems()
    {
        _storageSystem = new StorageSystem(resourceDataList, startingInventory);
        
        _researchSystem = new ResearchSystem(availableResearchNodes);
        
    
        var testResearch = ServiceLocator.GetService<ResearchSystem>();
        if (testResearch != null)
        {
            var allResearch = testResearch.GetAllResearchStatus();
            Debug.Log($"ResearchSystem creado con {allResearch.Count} investigaciones registradas");
        }
    }

    private void SetupTestEnvironment()
    {
        StorageSystem storage = WorldManager.Instance.Player.StorageSystem;
    
        storage.AddResource(ResourceType.Sandit, 0);
        storage.AddResource(ResourceType.Batee, 100);
        storage.AddResource(ResourceType.Paladium, 0);
        storage.AddResource(ResourceType.Frostice, 0);
        storage.AddResource(ResourceType.Magmavite, 0);
        
    }
    private void LogInitialState()
    {
        if (_researchSystem == null) _researchSystem = ServiceLocator.GetService<ResearchSystem>();
        try
        {
            var allResearch = _researchSystem.GetAllResearchStatus();
        
           // Debug.Log("ESTADO DE INVESTIGACIONES");
           // Debug.Log($"Total de investigaciones: {allResearch.Count}");
        
            foreach (var researchStatus in allResearch)
            {
                var researchNode = _researchSystem.GetResearch(researchStatus.Key);
                //Debug.Log($"- {researchStatus.Key}: {researchStatus.Value} (Node: {researchNode != null})");
            }

            // Verificar recursos
            StorageSystem storage = WorldManager.Instance.Player.StorageSystem;
            var resources = storage.GetAllResources();
           // Debug.Log("=== RECURSOS DISPONIBLES ===");
            foreach (var resource in resources)
            {
               // Debug.Log($"- {resource.Key}: {resource.Value}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al verificar estado: {e.Message}");
        }
    }
}
}