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

        // Eventos estáticos para notificar a todos los planetas sobre mejoras globales
        public static event Action<string, float> OnGlobalImprovementAdded;

        // Lista estática de mejoras globales acumuladas
        private static Dictionary<string, float> _globalImprovements = new Dictionary<string, float>();

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            OnGlobalImprovementAdded += HandleGlobalImprovementAdded;
        }

        private void Start()
        {
            ApplyExistingGlobalImprovements();
        }

        private void OnDestroy()
        {
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

            var gameTime = ServiceLocator.GetService<IGameTime>();
            _stateManager = new PlanetStateManager(gameTime);
            _stateManager.SetState(new BuildingState(this, gameTime));
        }

        // Resto de los métodos de mejoras se mantienen igual...
        private void ApplyExistingGlobalImprovements()
        {
            foreach (var improvement in _globalImprovements)
            {
                AddImprovement(improvement.Key, improvement.Value);
            }
        }

        public void AddImprovement(string improvementType, float percentage)
        {
            if (!_improvementPercentages.ContainsKey(improvementType))
            {
                _improvementPercentages[improvementType] = 0f;
            }

            _improvementPercentages[improvementType] += percentage;
            RecalculateProduction();
        }

        private void RecalculateProduction()
        {
            for (int i = 0; i < ResourcePerCycle.Length; i++)
            {
                ResourcePerCycle[i] = _baseResourcePerCycle[i];
            }

            float totalImprovement = GetTotalImprovementPercentage();

            if (totalImprovement > 0f)
            {
                for (int i = 0; i < ResourcePerCycle.Length; i++)
                {
                    float baseValue = _baseResourcePerCycle[i];
                    float newValue = baseValue * (1 + totalImprovement / 100f);
                    ResourcePerCycle[i] = Mathf.RoundToInt(newValue);
                }
            }

            Debug.Log($"Planet {Name}: Total improvement {totalImprovement}%, New production: {ResourcePerCycle[0]}");
        }

        public float GetTotalImprovementPercentage()
        {
            float total = 0f;
            foreach (var improvement in _improvementPercentages.Values)
            {
                total += improvement;
            }
            return total;
        }

        public float GetImprovementPercentage(string improvementType)
        {
            return _improvementPercentages.ContainsKey(improvementType) ?
                   _improvementPercentages[improvementType] : 0f;
        }

        public static void AddGlobalImprovement(string improvementType, float percentage)
        {
            Debug.Log($"=== ADDING GLOBAL IMPROVEMENT: {improvementType} {percentage}% ===");

            if (!_globalImprovements.ContainsKey(improvementType))
            {
                _globalImprovements[improvementType] = 0f;
            }
            _globalImprovements[improvementType] += percentage;

            Debug.Log($"Total {improvementType} improvement: {_globalImprovements[improvementType]}%");
            OnGlobalImprovementAdded?.Invoke(improvementType, percentage);
        }

        private void HandleGlobalImprovementAdded(string improvementType, float percentage)
        {
            AddImprovement(improvementType, percentage);
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