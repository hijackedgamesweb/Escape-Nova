using Code.Scripts.Core.Systems.Resources;

namespace Code.Scripts.Core.Systems.Research
{
    // ResearchInitializer.cs
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
    
    [Header("Test Resources")]
    [SerializeField] private int initialWood = 100;
    [SerializeField] private int initialStone = 100;

    private void Start()
    {
        InitializeSystems();
        SetupTestEnvironment();
        LogInitialState();
    }

    // ResearchInitializer.cs (Modificaciones en InitializeSystems)

    private void InitializeSystems()
    {
        StorageSystem storageSystem = new StorageSystem(resourceDataList, startingInventory);
        Debug.Log($"StorageSystem creado con {resourceDataList?.Count ?? 0} recursos");
        
        ResearchSystem researchSystem = new ResearchSystem(availableResearchNodes);
        researchSystem.InitializeDependencies(); 
    
        var testResearch = ServiceLocator.GetService<ResearchSystem>();
        if (testResearch != null)
        {
            var allResearch = testResearch.GetAllResearchStatus();
            Debug.Log($"ResearchSystem creado con {allResearch.Count} investigaciones registradas");
        }
    }

    private void SetupTestEnvironment()
    {
        StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
    
        // Añadir recursos iniciales para testear
        storage.AddResource(ResourceType.Arena, 200);
        storage.AddResource(ResourceType.Piedra, 100); 
    
        storage.AddInventoryItem("Lanza", 20);
        
        Debug.Log($"Recursos iniciales: 200 Madera, 100 Piedra");
    }
    private void LogInitialState()
    {
        try
        {
            ResearchSystem research = ServiceLocator.GetService<ResearchSystem>();
            var allResearch = research.GetAllResearchStatus();
        
            Debug.Log("ESTADO DE INVESTIGACIONES");
            Debug.Log($"Total de investigaciones: {allResearch.Count}");
        
            foreach (var researchStatus in allResearch)
            {
                var researchNode = research.GetResearch(researchStatus.Key);
                Debug.Log($"- {researchStatus.Key}: {researchStatus.Value} (Node: {researchNode != null})");
            }

            // Verificar recursos
            StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
            var resources = storage.GetAllResources();
            Debug.Log("=== RECURSOS DISPONIBLES ===");
            foreach (var resource in resources)
            {
                Debug.Log($"- {resource.Key}: {resource.Value}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al verificar estado: {e.Message}");
        }
    }

    // Metodos para testing desde el inspector
    [ContextMenu("Test Wood Capacity Research")]
    public void TestWoodCapacityResearch()
    {
        ResearchSystem research = ServiceLocator.GetService<ResearchSystem>();
        
        if (research.CanStartResearch("wood_capacity_1"))
        {
            research.StartResearch("wood_capacity_1");
            Debug.Log("Investigación de madera iniciada!");
        }
        else
        {
            Debug.Log("No se puede iniciar investigación de madera. Verificar recursos.");
        }
    }

    [ContextMenu("Test Stone Processing Research")]
    public void TestStoneProcessingResearch()
    {
        ResearchSystem research = ServiceLocator.GetService<ResearchSystem>();
        
        if (research.CanStartResearch("stone_processing"))
        {
            research.StartResearch("stone_processing");
            Debug.Log("Investigación de piedra iniciada!");
        }
        else
        {
            Debug.Log("No se puede iniciar investigación de piedra. Verificar recursos.");
        }
    }

    [ContextMenu("Check All Research Status")]
    public void CheckAllResearchStatus()
    {
        ResearchSystem research = ServiceLocator.GetService<ResearchSystem>();
        var allResearch = research.GetAllResearchStatus();
        
        Debug.Log("ESTADO ACTUAL DE INVESTIGACIONES");
        foreach (var researchStatus in allResearch)
        {
            var researchNode = research.GetResearch(researchStatus.Key);
            string status = researchStatus.Value.ToString();
            string progress = (research.GetResearchProgress(researchStatus.Key) * 100).ToString("F1");
            
            Debug.Log($"{researchNode.displayName}: {status} ({progress}%)");
        }
    }
}
}