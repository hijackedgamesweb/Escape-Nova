using System;
using System.Collections.Generic;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows.SubPanels
{
    public class SelectCivilizationPanel : MonoBehaviour
    {
        [SerializeField] List<GameObject> _civilizationImages;
        [SerializeField] GameObject _civilizationButton;
        CivilizationManager _civilizationManager;
        private List<Civilization> _pendingCivilizations = new();
        
        private void Start()
        {
            _civilizationManager = ServiceLocator.GetService<CivilizationManager>();

            foreach (var civilization in _civilizationManager.GetCivilizations)
            {
                AddCivilizationButtons(civilization);
            }

            _civilizationManager.OnNewCivilizationDiscovered += AddCivilizationButtons;
        }

        private void OnEnable()
        {
            foreach (var civ in _pendingCivilizations)
                CreateButton(civ);

            _pendingCivilizations.Clear();
        }
        
        private void AddCivilizationButtons(Civilization civ, bool isNew = true)
        {
            if (!gameObject.activeInHierarchy)
            {
                _pendingCivilizations.Add(civ);
                return;
            }

            CreateButton(civ);
        }
        
        private void CreateButton(Civilization civ)
        {
            var buttonInstance = Instantiate(_civilizationButton, transform);
            buttonInstance.GetComponent<CivilizationSelectButton>()
                .Initialize(civ.CivilizationData);
        }
    }
}