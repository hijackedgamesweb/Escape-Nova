using System;
using Code.Scripts.Core.Managers;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades
{
    [Serializable]
    public class ProductionIncrementUpgrade : Upgrade
    {
        [SerializeField] private float productionIncrement;
        
        public override void ApplyUpgrade(Planet planet)
        {
            for (int i = 0; i < planet.ResourcePerCycle.Length; i++)
            {
                planet.ResourcePerCycle[i] += (int) (planet.ResourcePerCycle[i] * productionIncrement);
            }
            NotificationManager.Instance.CreateNotification(
                $"La producción de recursos del planeta {planet.Name} ha aumentado en un {productionIncrement * 100}%", 
                NotificationType.Info);
        }
    }
}