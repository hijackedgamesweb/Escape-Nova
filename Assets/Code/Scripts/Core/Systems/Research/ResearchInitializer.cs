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
    
    [Header("Test Resources")]
    [SerializeField] private int initialWood = 100;
    [SerializeField] private int initialStone = 100;
    
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
        }
    }

    private void SetupTestEnvironment()
    {
        StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
    
        storage.AddResource(ResourceType.Arena, 2500);
        storage.AddResource(ResourceType.Piedra, 2100); 
        storage.AddResource(ResourceType.Metal, 2100); 
        storage.AddResource(ResourceType.Hielo, 2100); 
        storage.AddResource(ResourceType.Fuego, 2100); 
    }
    private void LogInitialState()
    {
        if (_researchSystem == null) _researchSystem = ServiceLocator.GetService<ResearchSystem>();
        try
        {
            var allResearch = _researchSystem.GetAllResearchStatus();
        
        
            foreach (var researchStatus in allResearch)
            {
                var researchNode = _researchSystem.GetResearch(researchStatus.Key);
                Debug.Log($"- {researchStatus.Key}: {researchStatus.Value} (Node: {researchNode != null})");
            }

            StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
            var resources = storage.GetAllResources();
            foreach (var resource in resources)
            {
            }
        }
        catch (System.Exception e)
        {
        }
    }

    [ContextMenu("Test Wood Capacity Research")]
    public void TestWoodCapacityResearch()
    {
        ResearchSystem research = ServiceLocator.GetService<ResearchSystem>();
        
        if (research.CanStartResearch("wood_capacity_1"))
        {
            research.StartResearch("wood_capacity_1");
        }
        else
        {
        }
    }

    [ContextMenu("Test Stone Processing Research")]
    public void TestStoneProcessingResearch()
    {
        ResearchSystem research = ServiceLocator.GetService<ResearchSystem>();
        
        if (research.CanStartResearch("stone_processing"))
        {
            research.StartResearch("stone_processing");
        }
        else
        {
        }
    }

    [ContextMenu("Check All Research Status")]
    public void CheckAllResearchStatus()
    {
        ResearchSystem research = ServiceLocator.GetService<ResearchSystem>();
        var allResearch = research.GetAllResearchStatus();
        
        foreach (var researchStatus in allResearch)
        {
            var researchNode = research.GetResearch(researchStatus.Key);
            string status = researchStatus.Value.ToString();
            string progress = (research.GetResearchProgress(researchStatus.Key) * 100).ToString("F1");
        }
    }
}
}