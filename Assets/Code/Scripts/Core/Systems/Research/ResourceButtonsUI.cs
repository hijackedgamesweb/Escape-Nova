using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;
using ResourceType = Code.Scripts.Core.Systems.Resources.ResourceType;

public class ResourceButtonsUI : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button addWoodButton;
    [SerializeField] private Button addStoneButton;
    [SerializeField] private Button resetResourcesButton;

    [Header("Resource Amounts")]
    [SerializeField] private int woodAmount = 100;
    [SerializeField] private int stoneAmount = 50;

    private StorageSystem _storageSystem;
    private bool _isInitialized = false;

    private void Start()
    {
        StartCoroutine(InitializeDelayed());
    }

    private IEnumerator InitializeDelayed()
    {
        
        yield return new WaitForSeconds(0.5f);
        
        int maxAttempts = 10;
        int attempts = 0;
        
        while (_storageSystem == null && attempts < maxAttempts)
        {
            attempts++;
            try
            {
                _storageSystem = ServiceLocator.GetService<StorageSystem>();
            }
            catch (System.Exception e)
            {
            }
            
            if (_storageSystem == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        if (_storageSystem == null)
        {
            yield break;
        }
        
        // Configurar botones
        if (addWoodButton != null)
        {
            addWoodButton.onClick.RemoveAllListeners();
            addWoodButton.onClick.AddListener(() => AddResource(ResourceType.Sandit, woodAmount));
        }
        else
        {
        }
            
        if (addStoneButton != null)
        {
            addStoneButton.onClick.RemoveAllListeners();
            addStoneButton.onClick.AddListener(() => AddResource(ResourceType.Batee, stoneAmount));
        }
        else
        {
        }
            
        if (resetResourcesButton != null)
        {
            resetResourcesButton.onClick.RemoveAllListeners();
            resetResourcesButton.onClick.AddListener(ResetResources);
        }
        else
        {
        }
        
        _isInitialized = true;
    }

    private void AddResource(ResourceType resourceType, int amount)
    {
        if (!_isInitialized || _storageSystem == null)
        {
            return;
        }
        
        bool success = _storageSystem.AddResource(resourceType, amount);
        if (success)
        {
            
            // Verificar que se añadió
            int newAmount = _storageSystem.GetResourceAmount(resourceType);
        }
        else
        {
        }
    }

    private void ResetResources()
    {
        if (!_isInitialized || _storageSystem == null)
        {
            return;
        }

        int currentWood = _storageSystem.GetResourceAmount(ResourceType.Sandit);
        int currentStone = _storageSystem.GetResourceAmount(ResourceType.Batee);
        _storageSystem.AddResource(ResourceType.Sandit, 200 - currentWood);
        _storageSystem.AddResource(ResourceType.Batee, 100 - currentStone);
        
    }

    [ContextMenu("Force Initialize")]
    public void ForceInitialize()
    {
        StartCoroutine(InitializeDelayed());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ForceInitialize();
        }
    }
}