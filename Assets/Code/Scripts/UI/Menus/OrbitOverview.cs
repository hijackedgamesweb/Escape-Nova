using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.SaveLoad.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Common;
using Code.Scripts.UI.Menus.BuildingMenuPanel;
using Code.Scripts.UI.Windows;
using Newtonsoft.Json.Linq;

namespace Code.Scripts.UI.Menus
{
    public class OrbitOverview : MonoBehaviour, ISaveable
    {
        [SerializeField] private SolarSystem _solarSystem;
        [SerializeField] private Sprite _planetSlotSprite;
        [SerializeField] private Sprite _orbitCircleSprite; 
        [SerializeField] private RectTransform _orbitContainer;
        [SerializeField] private float orbitRadiusStep = 150f;
        [SerializeField] private PlanetListInitializer _planetListInitializer;
        [SerializeField] private SateliteListInitializer _sateliteListInitializer;
        [SerializeField] private PerfectViewScreen _perfectViewScreen;
        private List<List<GameObject>> _planetSlots = new List<List<GameObject>>();
        
        private ConstructionType currentConstructionType = ConstructionType.Planet;
        
        private bool orbitInitialized = false;
        private void Awake()
        {
            ServiceLocator.RegisterService<OrbitOverview>(this);
        }
        
        private void Start()
        {
            if (!orbitInitialized)
                DrawOrbit(0);
            ConstructionPanelManager.OnConstructionTypeChanged += OnConstructionTypeChanged;
            if (_solarSystem != null)
            {
                _solarSystem.OnPlanetRemoved += DeletePlanetFromOverview;
            }
        }
        private void OnDestroy()
        {
            ConstructionPanelManager.OnConstructionTypeChanged -= OnConstructionTypeChanged;
            if (_solarSystem != null)
            {
                _solarSystem.OnPlanetRemoved -= DeletePlanetFromOverview;
            }
        }

        private void OnConstructionTypeChanged(ConstructionType obj)
        {
            currentConstructionType = obj;
        }

        private void PlanetSlotInitializer(GameObject planetSlot, int orbitIndex, int positionInOrbit)
        {
            planetSlot.AddComponent<Button>();
            planetSlot.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (currentConstructionType == ConstructionType.Planet)
                {
                    PlanetDataSO planetDataSo = _planetListInitializer.GetCurrentPlanetData();
                    
                    if (planetDataSo == null)
                    {
                        return;
                    }
                    bool consumed = _planetListInitializer.ConsumeResourcesForPlanet(planetDataSo);
                    if (consumed)
                    {
                        _solarSystem.AddPlanet(orbitIndex, positionInOrbit, planetDataSo);
                        AddPlanetToOverview(orbitIndex, positionInOrbit, planetDataSo);
                        _planetListInitializer.ClearSelection();
                        UIManager.Instance.ShowScreen<InGameScreen>();
                    }
                    else
                    {
                    }
                }
            });
        }
        
        private void AddPlanetToOverview(int orbitIndex, int positionInOrbit, PlanetDataSO planetDataSo)
        {
            GameObject planetSlot = _planetSlots[orbitIndex][positionInOrbit];
            Image img = planetSlot.GetComponent<Image>();
            img.sprite = planetDataSo.sprite;
            img.color = Color.white;
            planetSlot.GetComponent<Button>().onClick.RemoveAllListeners();
            planetSlot.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (currentConstructionType == ConstructionType.Satelite)
                {
                    SateliteDataSO sateliteDataSo = _sateliteListInitializer.GetCurrentSateliteData();
                    if (sateliteDataSo == null) return;
                    Planet targetPlanet = _solarSystem.Planets[orbitIndex][positionInOrbit];
                    if (targetPlanet != null && !targetPlanet.CanAddSatelite())
                    {
                        NotificationManager.Instance.CreateNotification($"Orbit full on {targetPlanet.Name}", NotificationType.Warning);
                        return;
                    }
                    bool consumed = _sateliteListInitializer.ConsumeResourcesForSatelite(sateliteDataSo);
                    if (consumed)
                    {
                        _solarSystem.AddSateliteToPlanet(orbitIndex, positionInOrbit, sateliteDataSo);
                        _sateliteListInitializer.ClearSelection();
                        UIManager.Instance.ShowScreen<InGameScreen>();
                    }
                    else
                    {
                        NotificationManager.Instance.CreateNotification("Not enough resources!", NotificationType.Error);
                    }
                }
            });
        }
        
        private void DeletePlanetFromOverview(int orbitIndex, int positionInOrbit)
        {
            GameObject planetSlot = _planetSlots[orbitIndex][positionInOrbit];
            Image img = planetSlot.GetComponent<Image>();
            img.sprite = _planetSlotSprite;
            img.color = new Color(0f, 1f, 0f, 1f);
            PlanetSlotInitializer(planetSlot, orbitIndex, positionInOrbit);
        }
        
        public void AddNextOrbit()
        {
            int nextOrbitIndex = _planetSlots.Count;
            var totalOrbits = _solarSystem.planetsPerOrbit.Length;

            if (nextOrbitIndex < totalOrbits)
            {
                DrawOrbit(nextOrbitIndex);
            }
            else
            {
            }
        }
        
        [ContextMenu("Añadir nueva órbita")]
        public void Debug_AddOrbit()
        {
            AddNextOrbit();
        }

       private void DrawOrbit(int orbitIndex)
        {
            var planetsPerOrbit = _solarSystem.planetsPerOrbit;

            if (orbitIndex < 0 || orbitIndex >= planetsPerOrbit.Length)
            {
                return;
            }

            float orbitRadius = orbitRadiusStep * (orbitIndex + 1);

            GameObject orbitCircle = new GameObject($"OrbitCircle_{orbitIndex}", typeof(RectTransform), typeof(OrbitCircleUI));
            orbitCircle.transform.SetParent(_orbitContainer, false);

            var circle = orbitCircle.GetComponent<OrbitCircleUI>();
            circle.color = new Color(1f, 1f, 1f, 1f);
            circle.SetRadius(orbitRadius);
            circle.SetThickness(3f);
            circle.raycastTarget = false;

            RectTransform orbitRect = orbitCircle.GetComponent<RectTransform>();
            float diameter = orbitRadius * 2f;
            orbitRect.sizeDelta = new Vector2(diameter, diameter);
            orbitRect.anchoredPosition = Vector2.zero;

            int planetsInOrbit = planetsPerOrbit[orbitIndex];
            _planetSlots.Add(new List<GameObject>());

            for (int i = 0; i < planetsInOrbit; i++)
            {
                float angle = (i * (360f / planetsInOrbit)) + 90f;
                float rad = angle * Mathf.Deg2Rad;

                GameObject planetSlot = new GameObject($"PlanetSlot_{orbitIndex}_{i}", typeof(RectTransform), typeof(Image));
                planetSlot.transform.SetParent(_orbitContainer, false);

                Image img = planetSlot.GetComponent<Image>();
                
                
                img.sprite = _planetSlotSprite;
                img.SetNativeSize();
                img.transform.localScale = Vector3.one * 0.1f;
                
                if (_solarSystem.Planets[orbitIndex][i] != null)
                {
                    planetSlot.AddComponent<Button>();
                    img.sprite = _solarSystem.Planets[orbitIndex][i].GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    img.color = new Color(0f, 1f, 0f, 1f);
                    PlanetSlotInitializer(planetSlot, orbitIndex, i);
                }


                Vector3 newPos = new Vector3(orbitRadius * Mathf.Cos(rad), orbitRadius * Mathf.Sin(rad), 0f);
                planetSlot.GetComponent<RectTransform>().anchoredPosition = newPos;

                _planetSlots[orbitIndex].Add(planetSlot);
            }
        }

        public string GetSaveId()
        {
            return "OrbitOverview";
        }

        public JToken CaptureState()
        {
            var planets = new JObject();
            for (int orbitIndex = 0; orbitIndex < _planetSlots.Count; orbitIndex++)
            {
                var orbit = new JObject();

                for (int positionInOrbit = 0; positionInOrbit < _planetSlots[orbitIndex].Count; positionInOrbit++)
                {
                    var planetSlot = _planetSlots[orbitIndex][positionInOrbit];

                    
                    orbit[positionInOrbit.ToString()] = _solarSystem.Planets[orbitIndex][positionInOrbit] != null
                        ? _solarSystem.Planets[orbitIndex][positionInOrbit].GetComponent<Planet>().PlanetData.name
                        : "";
                    
                }

                planets[$"Orbit{orbitIndex + 1}"] = orbit;
            }
            
            JObject state = new JObject
            {
                ["OrbitCount"] = _planetSlots.Count,
                ["Planets"] = planets
            };
            return state;
        }

        public void RestoreState(JToken state)
        {
            if (state == null) return;

            int orbitCount = state["OrbitCount"]?.ToObject<int>()?? 0;
            orbitInitialized = true;
            for (int i = 0; i < orbitCount; i++)
            {
                AddNextOrbit();
            }
            var planetsData = (JObject)state["Planets"];
            for (int orbitIndex = 0; orbitIndex < orbitCount; orbitIndex++)
            {
                var orbitKey = $"Orbit{orbitIndex + 1}";
                if (planetsData[orbitKey] is JObject orbit)
                {
                    for (int positionInOrbit = 0; positionInOrbit < _planetSlots[orbitIndex].Count; positionInOrbit++)
                    {
                        var planetKey = positionInOrbit.ToString();
                        string planetDataName = orbit[planetKey]?.ToObject<string>() ?? "";

                        if (!string.IsNullOrEmpty(planetDataName))
                        {
                            PlanetDataSO planetDataSo = Resources.Load<PlanetDataSO>($"ScriptableObjects/Planets/{planetDataName}");
                            if (planetDataSo != null)
                            {
                                AddPlanetToOverview(orbitIndex, positionInOrbit, planetDataSo);
                            }
                        }
                    }
                }
            }
        }
    }
}