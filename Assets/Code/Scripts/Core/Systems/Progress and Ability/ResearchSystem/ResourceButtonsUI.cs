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
        Debug.Log("ResourceButtonsUI iniciando...");
        StartCoroutine(InitializeDelayed());
    }

    private IEnumerator InitializeDelayed()
    {
        Debug.Log("🕒 Esperando inicialización de StorageSystem...");
        
        // Esperar varios frames para que ResearchInitializer se ejecute primero
        yield return new WaitForSeconds(0.5f);
        
        int maxAttempts = 10;
        int attempts = 0;
        
        while (_storageSystem == null && attempts < maxAttempts)
        {
            attempts++;
            try
            {
                _storageSystem = ServiceLocator.GetService<StorageSystem>();
                Debug.Log($"Intento {attempts}: StorageSystem = {_storageSystem != null}");
            }
            catch (System.Exception e)
            {
                Debug.Log($"Intento {attempts}: {e.Message}");
            }
            
            if (_storageSystem == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        if (_storageSystem == null)
        {
            Debug.LogError($"❌ No se pudo obtener StorageSystem después de {maxAttempts} intentos");
            yield break;
        }

        Debug.Log("✅ StorageSystem obtenido correctamente");
        
        // Configurar botones
        if (addWoodButton != null)
        {
            addWoodButton.onClick.RemoveAllListeners();
            addWoodButton.onClick.AddListener(() => AddResource(ResourceType.Madera, woodAmount));
            Debug.Log("✅ Botón de madera configurado");
        }
        else
        {
            Debug.LogError("❌ AddWoodButton no asignado");
        }
            
        if (addStoneButton != null)
        {
            addStoneButton.onClick.RemoveAllListeners();
            addStoneButton.onClick.AddListener(() => AddResource(ResourceType.Piedra, stoneAmount));
            Debug.Log("✅ Botón de piedra configurado");
        }
        else
        {
            Debug.LogError("❌ AddStoneButton no asignado");
        }
            
        if (resetResourcesButton != null)
        {
            resetResourcesButton.onClick.RemoveAllListeners();
            resetResourcesButton.onClick.AddListener(ResetResources);
            Debug.Log("✅ Botón de reset configurado");
        }
        else
        {
            Debug.LogError("❌ ResetResourcesButton no asignado");
        }
        
        _isInitialized = true;
        Debug.Log("🎯 ResourceButtonsUI inicializado completamente");
    }

    private void AddResource(ResourceType resourceType, int amount)
    {
        if (!_isInitialized || _storageSystem == null)
        {
            Debug.LogError("❌ ResourceButtonsUI no está inicializado");
            return;
        }

        Debug.Log($"🔄 Intentando añadir {amount} de {resourceType}");
        
        bool success = _storageSystem.AddResource(resourceType, amount);
        if (success)
        {
            Debug.Log($"✅ Añadidos {amount} de {resourceType}");
            
            // Verificar que se añadió
            int newAmount = _storageSystem.GetResourceAmount(resourceType);
            Debug.Log($"📊 Nuevo total de {resourceType}: {newAmount}");
        }
        else
        {
            Debug.LogError($"❌ Error al añadir {resourceType}");
        }
    }

    private void ResetResources()
    {
        if (!_isInitialized || _storageSystem == null)
        {
            Debug.LogError("❌ ResourceButtonsUI no está inicializado");
            return;
        }

        Debug.Log("🔄 Reseteando recursos...");
        
        // Obtener recursos actuales
        int currentWood = _storageSystem.GetResourceAmount(ResourceType.Madera);
        int currentStone = _storageSystem.GetResourceAmount(ResourceType.Piedra);
        
        // Resetear a valores iniciales de testing
        _storageSystem.AddResource(ResourceType.Madera, 200 - currentWood);
        _storageSystem.AddResource(ResourceType.Piedra, 100 - currentStone);
        
        Debug.Log("✅ Recursos reseteados a: 200 Madera, 100 Piedra, 50 Metal");
    }

    // Método temporal para debug
    [ContextMenu("Force Initialize")]
    public void ForceInitialize()
    {
        Debug.Log("Forzando inicialización...");
        StartCoroutine(InitializeDelayed());
    }

    private void Update()
    {
        // Tecla R para forzar reinicialización
        if (Input.GetKeyDown(KeyCode.R))
        {
            ForceInitialize();
        }
    }
}