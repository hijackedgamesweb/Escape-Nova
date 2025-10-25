using System;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;

namespace Code.Scripts.UI.HUD
{
    public class StorageHUD : MonoBehaviour
    {
        private StorageSystem _storageSystem;
        [SerializeField] private TMP_Text _woodText;
        [SerializeField] private TMP_Text _stoneText;
        [SerializeField] private TMP_Text _metalText;
        private void Start()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            
            _storageSystem.OnStorageUpdated += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            _woodText.text = _storageSystem.GetResourceAmount(ResourceType.Madera).ToString();
            _stoneText.text = _storageSystem.GetResourceAmount(ResourceType.Piedra).ToString();
            _metalText.text = _storageSystem.GetResourceAmount(ResourceType.Metal).ToString();
        }
    }
}