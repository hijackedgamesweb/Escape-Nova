using System;
using System.Collections.Generic;
using Code.Scripts.Camera;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Behaviour;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Core.World.ConstructableEntities.States;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.Core.World.ConstructableEntities
{
    public class Planet : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private PlanetDataSO _planetData;
        public PlanetDataSO PlanetData => _planetData;
        public int[] ResourcePerCycle { get; private set; }
        private int[] _baseResourcePerCycle;
        public List<ResourceType> ProducibleResources { get; private set; }
        public List<Satelite> Satelites { get; private set; } = new List<Satelite>();
        public int TimeToBuild { get; private set; }
        public int OrbitIndex { get; set; }
        public int PlanetIndex { get; set; }
        public string Name { get; private set; }
        
        public Civilization Owner { get; private set; }
        public Civilization Aggressor { get; private set; }
        private PlanetBehaviourRunner _behaviourRunner;
        public BaseBehaviour AssociatedAI { get; set; }
        public event Action<Civilization> OnOwnerChanged;
        
        public bool IsDestroyed { get; private set; } = false;
        public bool IsConquered 
        {
            get 
            {
                if (_behaviourRunner != null && _behaviourRunner.enabled && _behaviourRunner.PlanetGraph != null)
                {
                    return _behaviourRunner.PlanetGraph.IsCurrentState(_behaviourRunner.PlanetGraph.ConqueredState);
                }
                return false;
            }
        }

        PlanetStateManager _stateManager;
        private Dictionary<string, float> _improvementPercentages = new Dictionary<string, float>();
        public static event Action<string, float> OnGlobalImprovementAdded;
        private static Dictionary<string, float> _globalImprovements = new Dictionary<string, float>();

        private int _satelliteSlots;
        private bool[] _occupiedSatelliteSlots;
        
        private const float BASE_SATELLITE_DISTANCE = 0.6f;
        private const float SATELLITE_SPEED = 50f;

        private Animator _animator;
        
        public event Action<float> OnConstructionProgress;
        public event Action OnConstructionCompleted;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            OnGlobalImprovementAdded += HandleGlobalImprovementAdded;
        }

        private void Start()
        {
            ApplyExistingGlobalImprovements();
        }

        private void OnEnable()
        {
            SystemEvents.OnWarDeclaredToPlayer += HandleWarDeclaredGlobal;
        }
        
        private void OnDestroy()
        {
            OnGlobalImprovementAdded -= HandleGlobalImprovementAdded;
            SystemEvents.OnWarDeclaredToPlayer -= HandleWarDeclaredGlobal;
            _stateManager?.SetState(null);
        }

        public static float GetGlobalImprovement(string improvementType)
        {
            _globalImprovements.TryGetValue(improvementType, out float value);
            return value;
        }

       public void InitializePlanet(PlanetDataSO data, int orbit, int positionInOrbit)
        {
            _planetData = data;
            _spriteRenderer.sprite = data.sprite;
            Name = data.constructibleName;
            this.transform.localScale = Vector3.one * data.size;
            _behaviourRunner = GetComponent<PlanetBehaviourRunner>();
            
            if (_behaviourRunner != null)
            {
                _behaviourRunner.enabled = false;
            }
            
            if (_animator != null)
            {
                _animator.Play("Construyendo");
            
                float animSpeed = 1f / TimeToBuild; 
                _animator.speed = animSpeed;
            }
            
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
            
            _satelliteSlots = data.maxSatellites;
            _occupiedSatelliteSlots = new bool[_satelliteSlots];

            var gametTime = ServiceLocator.GetService<IGameTime>();
            _stateManager = new PlanetStateManager(gametTime, this);
            var buildingState = new BuildingState(this, gametTime);
            buildingState.OnProgressUpdated += HandleBuildingProgress;
            _stateManager.SetState(buildingState);

            RecalculateProduction(); 
        }
        private void RecalculateProduction()
        {
            if (ResourcePerCycle == null || _baseResourcePerCycle == null) return;

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
        public void HandleBuildingProgress(float progress)
        {
            OnConstructionProgress?.Invoke(progress);
        
            if (progress >= 1f)
            {
                OnConstructionCompleted?.Invoke();
            
                if (_animator != null) _animator.speed = 1f;
            
                if (_behaviourRunner != null)
                {
                    Debug.Log("_behaviourRunner es true");
                    _behaviourRunner.enabled = true;
                }
            }
        }

        public int GetResourceProductionOfType(ResourceType type)
        {
            if (IsConquered) return 0;

            for (int i = 0; i < ProducibleResources.Count; i++)
            {
                if (ProducibleResources[i] == type)
                {
                    return ResourcePerCycle[i];
                }
            }
            return 0;
        }


        public void CompleteConstructionInstantly()
        {
            OnConstructionCompleted?.Invoke();
        }
        
        private void HandleWarDeclaredGlobal(Civilization civThatDeclaredWar)
        {
            if (Owner != null && civThatDeclaredWar == Owner)
            {
                Debug.Log($"<color=red>[PLANET] {Name}: Mi dueño ({Owner.CivilizationData.Name}) entra en guerra con el JUGADOR.</color>");
                DeclareWar(Owner); 
            }
        }
        
        public bool CanBeDestroyedByPlayer(out string reason)
        {
            reason = string.Empty;
            if (Owner != null)
            {
                reason = $"Cannot destroy planet: Inhabited by {Owner.CivilizationData.Name}!";
                return false;
            }

            if (AssociatedAI != null && AssociatedAI._isAtWarWithPlayer)
            {
                reason = "Cannot destroy planet: The civilization has declared WAR on you!";
                return false;
            }
            if (_behaviourRunner == null || !_behaviourRunner.enabled || _behaviourRunner.PlanetGraph == null)
            {
                return true;
            }

            var fsm = _behaviourRunner.PlanetGraph;
            if (fsm.IsCurrentState(fsm.ConflictState))
            {
                reason = "Cannot destroy planet during active WAR.";
                return false;
            }
    
            if (fsm.IsCurrentState(fsm.ConqueredState))
            {
                reason = "Cannot destroy a CONQUERED planet.";
                return false;
            }

            return true;
        }
        
        public void DestroyPlanet()
        {
            IsDestroyed = true;
            bool fsmActive = _behaviourRunner != null && _behaviourRunner.enabled;
            Debug.Log($"<color=cyan>[CHIVATO 1] DestroyPlanet llamado. IsDestroyed=TRUE. ¿FSM Encendida?: {fsmActive}</color>");
            
            if (_behaviourRunner == null || !_behaviourRunner.enabled)
            {
                Debug.Log($"[{Name}] Borrando durante construcción (sin animación de FSM).");
                var solarSystem = ServiceLocator.GetService<SolarSystem>();
                if (solarSystem != null)
                {
                    solarSystem.RemovePlanet(OrbitIndex, PlanetIndex);
                }
                return;
            }
        }
        
        public void EstablishContact(Civilization newOwner)
        {
            if (newOwner == null) 
            {
                Debug.LogWarning($"[{name}] Se intentó establecer contacto con una civilización NULL.");
                return;
            }

            Owner = newOwner;
            if (newOwner.AIController is BaseBehaviour ai)
            {
                AssociatedAI = ai;
            }
            else
            {
                AssociatedAI = null;
            }

            OnOwnerChanged?.Invoke(Owner);
        }

        public void DeclareWar(Civilization aggressor)
        {
            Aggressor = aggressor;
        }
        public void WinWar()
        {
            Aggressor = null;
        }
        public void LoseWar(Civilization conqueror)
        {
            Aggressor = null;
            EstablishContact(conqueror);
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

        public bool CanAddSatelite()
        {
            if (_occupiedSatelliteSlots == null) return false;
            for (int i = 0; i < _satelliteSlots; i++)
            {
                if (!_occupiedSatelliteSlots[i])
                {
                    return true;
                }
            }
            return false;
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
                UnityEngine.Camera.main.GetComponent<CameraController2D>()?.SetTarget(this.transform);
            
            var runner = GetComponent<Code.Scripts.Core.Systems.Behaviour.PlanetBehaviourRunner>();

            if (runner != null && runner.PlanetGraph != null)
            {
                if (runner.PlanetGraph.IsCurrentState(runner.PlanetGraph.ConflictState))
                {
                    var warManager = FindObjectOfType<WarUIManager>();
                    if (warManager != null && AssociatedAI != null)
                    {
                        warManager.OpenBattlePanelForPlanet(this, AssociatedAI);
                    }
                }
                else
                {
                    PlanetInfoPanel.Instance.ShowPanel(this);
                }
            }
        }

        public bool AddSatelite(string sateliteId)
        {
            int freeSlotIndex = -1;
            for (int i = 0; i < _satelliteSlots; i++)
            {
                if (!_occupiedSatelliteSlots[i])
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

            _occupiedSatelliteSlots[freeSlotIndex] = true;

            Satelite satelite = new Satelite();
            var sateliteDataArray = Resources.LoadAll<SateliteDataSO>("ScriptableObjects/Satelites");
            SateliteDataSO SateliteData = System.Array.Find(sateliteDataArray, data => data.name == sateliteId || data.constructibleName == sateliteId);
            satelite.InitializeSatelite(SateliteData, this);
            Satelites.Add(satelite);
            foreach (var upgrade in SateliteData.upgrades)
            {
                upgrade.ApplyUpgrade(this);
            }

            CreateSatelliteVisual(SateliteData, freeSlotIndex);
            
            return true;
        }

        public bool AddSatelite(SateliteDataSO sateliteDataSo)
        {
            int freeSlotIndex = -1;
            for (int i = 0; i < _satelliteSlots; i++)
            {
                if (!_occupiedSatelliteSlots[i])
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

            _occupiedSatelliteSlots[freeSlotIndex] = true;

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
            float fixedAngle = slotIndex * (360f / _satelliteSlots);

            orbitCtrl.Initialize(orbitRadius, SATELLITE_SPEED, fixedAngle);
        }

        public string GetSaveId()
        {
            return $"{OrbitIndex}_{PlanetIndex}";
        }

        public JToken CaptureState()
        {
            JObject state = new JObject();
        
            state["PlanetName"] = _planetData.constructibleName;
            state["OrbitIndex"] = OrbitIndex;
            state["PlanetIndex"] = PlanetIndex;

            state["State"] = _stateManager.GetCurrentState().GetType().AssemblyQualifiedName;
        
            JArray sateliteArray = new JArray();
            foreach (var satelite in Satelites)
                sateliteArray.Add(satelite.CaptureState());
        
            state["Satelites"] = sateliteArray;
        
            JObject improvementsObj = new JObject();
            foreach (var improvement in _improvementPercentages)
                improvementsObj[improvement.Key] = improvement.Value;
        
            state["Improvements"] = improvementsObj;
        
            return state;
        }


        public void RestoreState(JToken state)
        {
            JObject obj = state as JObject;
            string stateName = obj["State"].ToObject<string>();
            _stateManager.SetStateByName(stateName);

            JArray sateliteArray = (JArray)obj["Satelites"];

            foreach (JObject satState in sateliteArray)
            {
                Satelite sat = new Satelite();
                sat.RestoreState(satState);

                // Cargar SO desde Resources
                var satSO = Resources.Load<SateliteDataSO>($"ScriptableObjects/Satelites/{sat.Name}");
                sat.InitializeSatelite(satSO, this);

                Satelites.Add(sat);
                AddSatelite(satSO); // también crea visual y upgrades
            }

            JObject improvementsObj = obj["Improvements"] as JObject;
            foreach (var improvementProperty in improvementsObj.Properties())
            {
                string improvementType = improvementProperty.Name;
                float percentage = improvementProperty.Value.ToObject<float>();
                _improvementPercentages[improvementType] = percentage;
            }

            RecalculateProduction();
        }
    }
}