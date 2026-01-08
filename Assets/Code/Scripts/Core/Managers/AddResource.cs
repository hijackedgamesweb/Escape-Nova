using System.Resources;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    
    public class AddResource : MonoBehaviour
    {
        [SerializeField] private StorageSystem _storageSystem;

        void Start()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
        }
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.F1))
            //{
            //    _storageSystem.AddResource(ResourceType.Batee, 10000000);
            //    _storageSystem.AddResource(ResourceType.Paladium, 10000000);
            //    _storageSystem.AddResource(ResourceType.Magmavite, 10000000);
            //    _storageSystem.AddResource(ResourceType.Sandit, 10000000);
            //    _storageSystem.AddResource(ResourceType.Frostice, 10000000);
            //}
        }
        
    }
}