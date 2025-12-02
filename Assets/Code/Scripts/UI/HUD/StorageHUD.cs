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

        [SerializeField] private TMP_Text _sandName;
        [SerializeField] private TMP_Text _stoneName;
        [SerializeField] private TMP_Text _metalName;
        [SerializeField] private TMP_Text _iceName;
        [SerializeField] private TMP_Text _fireName;

        private void Start()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            _storageSystem.OnStorageUpdated += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            _sandText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Sandit));
            _stoneText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Batee));
            _metalText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Paladium));
            _iceText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Frostice));
            _lavavagiText.text = NumberFormatter.FormatNumber(_storageSystem.GetResourceAmount(ResourceType.Magmavite));
        }

        private void OnDestroy()
        {
            if (_storageSystem != null)
            {
                _storageSystem.OnStorageUpdated -= UpdateUI;
            }
        }

        public void ShowResourceName(ResourceType type)
        {
            HideAllNames();

            switch (type)
            {
                case ResourceType.Sandit:
                    _sandName.gameObject.SetActive(true);
                    break;
                case ResourceType.Batee:
                    _stoneName.gameObject.SetActive(true);
                    break;
                case ResourceType.Paladium:
                    _metalName.gameObject.SetActive(true);
                    break;
                case ResourceType.Frostice:
                    _iceName.gameObject.SetActive(true);
                    break;
                case ResourceType.Magmavite:
                    _fireName.gameObject.SetActive(true);
                    break;
            }
        }

        public void HideAllNames()
        {
            _sandName.gameObject.SetActive(false);
            _stoneName.gameObject.SetActive(false);
            _metalName.gameObject.SetActive(false);
            _iceName.gameObject.SetActive(false);
            _fireName.gameObject.SetActive(false);
        }

    }
}