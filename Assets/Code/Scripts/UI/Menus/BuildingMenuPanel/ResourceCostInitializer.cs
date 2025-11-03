using System;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class ResourceCostInitializer : MonoBehaviour
    {
        [SerializeField] private ResourceCostPrefab _resourcePrefab;
        public void Initialize(ConstructibleDataSO constructibleData)
        {
            for (int i = 0; i < constructibleData.buildCostResources.Length; i++)
            {
                ResourceCostPrefab resourceItem = Instantiate(_resourcePrefab, transform);
                resourceItem.Initialize(constructibleData.buildCostResources[i].Icon, constructibleData.buildCostAmounts[i]);
            }
        }
    }
}