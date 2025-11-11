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
            planet.AddImprovement("Satelite", productionIncrement);
            
            NotificationManager.Instance.CreateNotification(
                $"La producción de recursos del planeta {planet.Name} ha aumentado en un {productionIncrement}%", 
                NotificationType.Info);
        }
    }
}