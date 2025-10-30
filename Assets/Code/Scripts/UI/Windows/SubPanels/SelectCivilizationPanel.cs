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
        private void Start()
        {
            _civilizationManager = ServiceLocator.GetService<CivilizationManager>();
            
            foreach (var civilization in _civilizationManager.GetCivilizations)
            {
                AddCivilizationButtons(civilization);
            }
            
            _civilizationManager.OnNewCivilizationDiscovered += AddCivilizationButtons;
        }
        
        private void AddCivilizationButtons(Civilization civilization)
        {
            var buttonInstance =Instantiate(_civilizationButton, transform);
            buttonInstance.GetComponent<CivilizationSelectButton>().Initialize(civilization.CivilizationData);
            
        }
    }
}