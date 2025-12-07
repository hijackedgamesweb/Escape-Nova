using System;
using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.Objectives
{
    [Serializable]
    public class TotalProductionObjective : QuestObjective
    {
        [Header("Configuración")]
        [Tooltip("Lista de recursos cuya producción por ciclo se sumará.")]
        public List<ResourceType> targetResources;
        
        [Tooltip("La cantidad total de producción por ciclo requerida.")]
        public int requiredTotalAmount;

        private SolarSystem _solarSystem;

        public override void Initialize()
        {
            isCompleted = false;
            _solarSystem = ServiceLocator.GetService<SolarSystem>();
        }

        public override void CheckCompletion()
        {
            if (_solarSystem == null) return;
            int currentTotal = _solarSystem.GetTotalResourceProductionFor(targetResources);

            if (currentTotal >= requiredTotalAmount)
            {
                isCompleted = true;
                UnregisterEvents();
            }
        }
        public override void RegisterEvents()
        {
            ConstructionEvents.OnPlanetAdded += OnProductionSourceChanged;
            ConstructionEvents.OnPlanetRemoved += OnProductionSourceChanged;
        }

        public override void UnregisterEvents()
        {
            ConstructionEvents.OnPlanetAdded -= OnProductionSourceChanged;
            ConstructionEvents.OnPlanetRemoved -= OnProductionSourceChanged;
        }
        private void OnProductionSourceChanged(object obj)
        {
            CheckCompletion();
        }
    }
}