using System;
using System.Collections.Generic;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class ResourceCostInitializer : MonoBehaviour
    {
        [SerializeField] private ResourceCostPrefab _resourcePrefab;
        private List<ResourceCostPrefab> _resourceItems = new List<ResourceCostPrefab>();
        public void Initialize(ConstructibleDataSO constructibleData, int[] costPerResource)
        {
            for (int i = 0; i < constructibleData.buildCostResources.Length; i++)
            {
                ResourceCostPrefab resourceItem = Instantiate(_resourcePrefab, transform);
                resourceItem.Initialize(constructibleData.buildCostResources[i].Icon, costPerResource[i]);
                _resourceItems.Add(resourceItem);
            }
        }
        
        public void UpdateCosts(int[] newCosts)
        {
            for (int i = 0; i < _resourceItems.Count; i++)
            {
                _resourceItems[i].UpdateCosts(newCosts[i]);
            }
        }
        
    }
}