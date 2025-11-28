using System;
using System.Collections.Generic;
using Code.Scripts.Camera;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Core.World.ConstructableEntities.States;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus;
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
        private Dictionary<string, float> _improvementPercentages = new Dictionary<string, float>();
        public static event Action<string, float> OnGlobalImprovementAdded;
        private static Dictionary<string, float> _globalImprovements = new Dictionary<string, float>();

        private const int MAX_SLOTS = 3; 
        private const float BASE_SATELLITE_DISTANCE = 0.6f;
        private const float SATELLITE_SPEED = 50f;

        private bool[] _occupiedSlots = new bool[MAX_SLOTS];
        public event Action<float> OnConstructionProgress;
        public event Action OnConstructionCompleted;

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
            _stateManager?.SetState(null);
        }

        public static float GetGlobalImprovement(string improvementType)
        {
            _globalImprovements.TryGetValue(improvementType, out float value);
            return value;
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
            var buildingState = new BuildingState(this, gametTime);
            buildingState.OnProgressUpdated += HandleBuildingProgress;
            _stateManager.SetState(buildingState);
        }
        private void HandleBuildingProgress(float progress)
        {
            OnConstructionProgress?.Invoke(progress);
            
            if (progress >= 1f)
            {
                OnConstructionCompleted?.Invoke();
            }
        }

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

            float oldValue = _improvementPercentages[improvementType];
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
            if (!_globalImprovements.ContainsKey(improvementType))
            {
                _globalImprovements[improvementType] = 0f;
            }
            _globalImprovements[improvementType] += percentage;
            OnGlobalImprovementAdded?.Invoke(improvementType, percentage);
        }

        private void HandleGlobalImprovementAdded(string improvementType, float percentage)
        {
            AddImprovement(improvementType, percentage);
        }

        public void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; 

            Debug.Log("Planet clicked: " + this.name);
            if(UnityEngine.Camera.main != null)
                UnityEngine.Camera.main.GetComponent<CameraController2D>().SetTarget(this.transform);
            
            PlanetInfoPanel.Instance.ShowPanel(this);
        }

        public bool AddSatelite(SateliteDataSO sateliteDataSo)
        {
            int freeSlotIndex = -1;
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (!_occupiedSlots[i])
                {
                    freeSlotIndex = i;
                    break;
                }
            }

            if (freeSlotIndex == -1)
            {
                NotificationManager.Instance.CreateNotification($"Max satellites reached for {Name}", NotificationType.Warning);
                return false;
            }

            _occupiedSlots[freeSlotIndex] = true;

            Satelite satelite = new Satelite();
            satelite.InitializeSatelite(sateliteDataSo, this);
            Satelites.Add(satelite);
            foreach (var upgrade in sateliteDataSo.upgrades)
            {
                upgrade.ApplyUpgrade(this);
            }

            CreateSatelliteVisual(sateliteDataSo, freeSlotIndex);
            
            return true;
        }

        private void CreateSatelliteVisual(SateliteDataSO data, int slotIndex)
        {
            GameObject satObj = new GameObject($"{data.name}_Visual_{slotIndex}");
            satObj.transform.SetParent(this.transform);
            satObj.transform.localScale = Vector3.one * (data.size > 0 ? data.size : 0.3f);

            SpriteRenderer sr = satObj.AddComponent<SpriteRenderer>();
            sr.sprite = data.sprite;
            

            if (_spriteRenderer != null) 
            {
                sr.sortingOrder = _spriteRenderer.sortingOrder + 1;
            }

            SatelliteOrbitController orbitCtrl = satObj.AddComponent<SatelliteOrbitController>();
            
            float planetRadius = 1f;
            if (_spriteRenderer != null && _spriteRenderer.sprite != null)
            {
                planetRadius = _spriteRenderer.bounds.extents.x;
            }
            
            float orbitRadius = planetRadius + BASE_SATELLITE_DISTANCE;
            float fixedAngle = slotIndex * (360f / MAX_SLOTS);

            orbitCtrl.Initialize(orbitRadius, SATELLITE_SPEED, fixedAngle);
        }
    }
}