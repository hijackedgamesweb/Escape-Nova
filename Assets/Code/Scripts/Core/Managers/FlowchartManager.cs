using System;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Patterns.ServiceLocator;
using Fungus;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class FlowchartManager : MonoBehaviour
    {
        [SerializeField] private Flowchart _mippipFlowchart;
        [SerializeField] private Flowchart _akkiFlowchart;
        [SerializeField] private Flowchart _halxiFlowchart;
        [SerializeField] private Flowchart _skulgFlowchart;
        [SerializeField] private Flowchart _handoullFlowchart;

        private void Start()
        {
            ServiceLocator.GetService<CivilizationManager>().OnNewCivilizationDiscovered += OnNewCivilizationDiscovered;
        }

        private void OnNewCivilizationDiscovered(Civilization obj)
        {
            switch (obj.CivilizationData.Name)
            {
                case "Mippip":
                    _mippipFlowchart.SendFungusMessage("DiscoverMippip");
                    break;
                case "Akki":
                    _akkiFlowchart.gameObject.SetActive(true);
                    break;
                case "Halxi":
                    _halxiFlowchart.gameObject.SetActive(true);
                    break;
                case "Skulg":
                    _skulgFlowchart.gameObject.SetActive(true);
                    break;
                case "Handoull":
                    _handoullFlowchart.gameObject.SetActive(true);
                    break;
            }
        }
    }
}