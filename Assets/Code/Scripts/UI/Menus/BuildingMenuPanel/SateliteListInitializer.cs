using System.Collections.Generic;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class SateliteListInitializer : MonoBehaviour
    {
        [SerializeField] private List<SateliteDataSO> _sateliteDataSOs;
        [SerializeField] private SateliteListPrefab _sateliteListPrefab;
        
        private void Start()
        {
            foreach (var sateliteData in _sateliteDataSOs)
            {
                SateliteListPrefab sateliteItem = Instantiate(_sateliteListPrefab, transform);
                sateliteItem.Initialize(sateliteData);
            }
        }
    }
}