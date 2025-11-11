using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Camera;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Core.World.ConstructableEntities.States;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.Core.World.ConstructableEntities
{
    public class Planet : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        public int[] ResourcePerCycle { get; private set; }
        private int[] _baseResourcePerCycle;
        public List<ResourceType> ProducibleResources { get; private set; }
        public List<Satelite> Satelites { get; private set; } = new List<Satelite>();
        public int TimeToBuild { get; private set; }
        public int OrbitIndex { get; set; }
        public int PlanetIndex { get; set; }
        public string Name { get; private set; }

        PlanetStateManager _stateManager;

        // Diccionario para acumular mejoras por tipo
        private Dictionary<string, float> _improvementPercentages = new Dictionary<string, float>();

        // Eventos est�ticos para notificar a todos los planetas sobre mejoras globales
        public static event Action<string, float> OnGlobalImprovementAdded;

        // Lista est�tica de mejoras globales acumuladas
        private static Dictionary<string, float> _globalImprovements = new Dictionary<string, float>();

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            // Suscribirse a eventos globales
            OnGlobalImprovementAdded += HandleGlobalImprovementAdded;
        }

        private void Start()
        {
            // Aplicar todas las mejoras globales existentes a este planeta
            ApplyExistingGlobalImprovements();
        }

        private void OnDestroy()
        {
            // Desuscribirse de eventos globales
            OnGlobalImprovementAdded -= HandleGlobalImprovementAdded;
        }

        public void InitializePlanet(PlanetDataSO data, int orbit, int positionInOrbit)
        {
            _spriteRenderer.sprite = data.sprite;
            Name = data.constructibleName;
            this.transform.localScale = Vector3.one * data.size;
            ResourcePerCycle = (int[])data.resourcePerCycle.Clone();
            _baseResourcePerCycle = (int[])data.resourcePerCycle.Clone();
            ProducibleResources = new List<ResourceType>();
            foreach (var resourceData in data.producibleResources)
            {
                ProducibleResources.Add(resourceData.Type);
            }
            TimeToBuild = data.timeToBuild;
            OrbitIndex = orbit;
            PlanetIndex = positionInOrbit;

            var gametTime = ServiceLocator.GetService<IGameTime>();
            _stateManager = new PlanetStateManager(gametTime);
            _stateManager.SetState(new BuildingState(this, gametTime));
        }

        // Aplicar mejoras globales existentes a este planeta
        private void ApplyExistingGlobalImprovements()
        {
            foreach (var improvement in _globalImprovements)
            {
                AddImprovement(improvement.Key, improvement.Value, false);
            }
        }

        // M�todo para aplicar mejoras espec�ficas a este planeta
        public void AddImprovement(string improvementType, float percentage, bool showLog = true)
        {
            if (!_improvementPercentages.ContainsKey(improvementType))
            {
                _improvementPercentages[improvementType] = 0f;
            }

            float oldValue = _improvementPercentages[improvementType];
            _improvementPercentages[improvementType] += percentage;

            if (showLog)
            {
                Debug.Log($"Planet {Name}: {improvementType} changed from {oldValue}% to {_improvementPercentages[improvementType]}%");
            }

            RecalculateProduction();
        }

        // Recalcula la producci�n bas�ndose en los valores base y las mejoras acumuladas
        private void RecalculateProduction()
        {
            // Resetear a los valores base
            for (int i = 0; i < ResourcePerCycle.Length; i++)
            {
                ResourcePerCycle[i] = _baseResourcePerCycle[i];
            }

            // Calcular el porcentaje total de todas las mejoras
            float totalImprovement = GetTotalImprovementPercentage();

            // Aplicar el porcentaje total al valor BASE
            if (totalImprovement > 0f)
            {
                for (int i = 0; i < ResourcePerCycle.Length; i++)
                {
                    float baseValue = _baseResourcePerCycle[i];
                    float newValue = baseValue * (1 + totalImprovement / 100f);
                    ResourcePerCycle[i] = Mathf.RoundToInt(newValue);
                }
            }

            // Mostrar resumen de mejoras
            string improvementsSummary = string.Join(", ", _improvementPercentages.Select(kvp => $"{kvp.Key}: {kvp.Value}%"));
            Debug.Log($"Planet {Name}: Base={_baseResourcePerCycle[0]}, Improvements={improvementsSummary}, Total={totalImprovement}%, Final={ResourcePerCycle[0]}");
        }

        // Obtener el porcentaje total de mejora
        public float GetTotalImprovementPercentage()
        {
            float total = 0f;
            foreach (var improvement in _improvementPercentages.Values)
            {
                total += improvement;
            }
            return total;
        }

        // Obtener el porcentaje de una mejora espec�fica
        public float GetImprovementPercentage(string improvementType)
        {
            return _improvementPercentages.ContainsKey(improvementType) ?
                   _improvementPercentages[improvementType] : 0f;
        }

        // M�todo est�tico para a�adir mejoras globales a todos los planetas
        public static void AddGlobalImprovement(string improvementType, float percentage)
        {
            Debug.Log($"=== ADDING GLOBAL IMPROVEMENT: {improvementType} {percentage}% ===");

            // Actualizar el registro global
            if (!_globalImprovements.ContainsKey(improvementType))
            {
                _globalImprovements[improvementType] = 0f;
            }
            _globalImprovements[improvementType] += percentage;

            Debug.Log($"Total {improvementType} improvement: {_globalImprovements[improvementType]}%");

            // Notificar a todos los planetas existentes
            OnGlobalImprovementAdded?.Invoke(improvementType, percentage);
        }

        // Manejar mejora global a�adida
        private void HandleGlobalImprovementAdded(string improvementType, float percentage)
        {
            AddImprovement(improvementType, percentage, false);
        }

        public void OnMouseDown()
        {
            Debug.Log("Planet clicked: " + this.name);
            UnityEngine.Camera.main.GetComponent<CameraController2D>().SetTarget(this.transform);
        }

        public void AddSatelite(SateliteDataSO sateliteDataSo)
        {
            Satelite satelite = new Satelite();
            satelite.InitializeSatelite(sateliteDataSo, this);
            Satelites.Add(satelite);
            foreach (var upgrade in sateliteDataSo.upgrades)
            {
                upgrade.ApplyUpgrade(this);
            }
        }
    }
}