using System;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Code.Scripts.UI.HUD
{
    public class StorageHUD : MonoBehaviour
    {
        private StorageSystem _storageSystem;
        [SerializeField] private TMP_Text _sandText;
        [SerializeField] private TMP_Text _iceText;
        [SerializeField] private TMP_Text _lavavagiText;
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
            _sandText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Arena));
            _stoneText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Piedra));
            _metalText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Metal));
            _iceText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Hielo));
            _lavavagiText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Fuego));
        }

        private void OnDestroy()
        {
            if (_storageSystem != null)
            {
                _storageSystem.OnStorageUpdated -= UpdateUI;
            }
        }
    }
}