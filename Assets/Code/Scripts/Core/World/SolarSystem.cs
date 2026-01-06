using System;
using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.ServiceLocator;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.World
{
    public class SolarSystem : MonoBehaviour, ISaveable
    {
        [Header("Orbits Settings")]
        [SerializeField] public int[] planetsPerOrbit;
        [SerializeField] private float orbitDistanceIncrement;
        [SerializeField] private float rotationSpeed = 20f;

        [Header("Planet Factory")]
        [SerializeField] private PlanetFactory planetFactory;
        [SerializeField] private PlanetDataSO[] planetDatas;
        
        [Header("UI Scroll Control")]
        [SerializeField] private ScrollRect planetListScrollRect;
        
        [SerializeField] private GameObject sun;

        private Transform sunPos;
        
        public List<List<Planet>> Planets = new();
        
        public event Action<int, int> OnPlanetRemoved;

        private bool _initialized = false;

        private void Awake()
        {
            for (int i = 0; i < planetsPerOrbit.Length; i++)
            {
                List<Planet> orbit = new List<Planet>();

                for (int j = 0; j < planetsPerOrbit[i]; j++)
                {
                    orbit.Add(null); 
                }

                Planets.Add(orbit);
            }
            ServiceLocator.RegisterService(this);
        }

       
        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            
            GameObject sunInstance = Instantiate(sun, Vector3.zero, Quaternion.identity, transform);
            sunPos = sunInstance.transform;
            if (sunInstance.GetComponent<SunInteractable>() == null)
            {
                sunInstance.AddComponent<SunInteractable>();
            }
            if (sunInstance.GetComponent<Collider2D>() == null)
            {
                sunInstance.AddComponent<CircleCollider2D>().radius = 2.5f;
            }
        }

        public Planet GetPlanet(int orbitIndex, int planetIndex)
        {
            if (orbitIndex < 0 || orbitIndex >= Planets.Count) return null;
            if (planetIndex < 0 || planetIndex >= Planets[orbitIndex].Count) return null;
            
            return Planets[orbitIndex][planetIndex];
        }

        public void ResetScrollPositionToTop()
        {
            if (planetListScrollRect == null)
            {
                return;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            planetListScrollRect.verticalNormalizedPosition = 1f;
        }

        public void AddPlanet(int orbit, int positionInOrbit, PlanetDataSO data)
        {
            if(Planets[orbit][positionInOrbit] != null && Planets[orbit][positionInOrbit].gameObject != null)
            {
                return;
            }
            Planet planet = planetFactory.CreatePlanet(Vector3.zero, data, transform, orbit, positionInOrbit);

            OrbitController orbitCtrl = planet.gameObject.AddComponent<OrbitController>();
            orbitCtrl.Initialize(planet, (orbit + 1) * orbitDistanceIncrement, positionInOrbit, planetsPerOrbit[orbit], rotationSpeed, orbit);
            NotificationManager.Instance.CreateNotification($"A planet named: {data.constructibleName} was added on the orbit {orbit + 1}", NotificationType.Info);
            Planets[orbit][positionInOrbit] = planet;
            ConstructionEvents.OnConstructibleCreated?.Invoke(data);
            ConstructionEvents.OnPlanetAdded?.Invoke(planet);
            var civManager = ServiceLocator.GetService<CivilizationManager>();
            if (civManager != null)
            {
                civManager.TryAssignPlanetToCivilization(planet);
            }
        }

        public void AddSateliteToPlanet(int orbitIndex, int positionInOrbit, SateliteDataSO sateliteDataSo)
        {
            Planet planet = Planets[orbitIndex][positionInOrbit];
            if (planet == null)
            {
                return;
            }
            bool success = planet.AddSatelite(sateliteDataSo);
            
            if (success)
            {
                ConstructionEvents.OnConstructibleCreated?.Invoke(sateliteDataSo);
                NotificationManager.Instance.CreateNotification($"A satellite named: {sateliteDataSo.name} was added to {planet.Name}", NotificationType.Info);
            }
        }

        public void RemovePlanet(int orbit, int positionInOrbit)
        {
            if (orbit < 0 || orbit >= Planets.Count || positionInOrbit < 0 || positionInOrbit >= Planets[orbit].Count)
            {
                return;
            }
            if (Planets[orbit][positionInOrbit] == null)
            {
                return;
            }

            Planet planet = Planets[orbit][positionInOrbit];
            if (planet.Owner != null)
            {
                NotificationManager.Instance.CreateNotification($"Cannot destroy {planet.Name}: inhabited by {planet.Owner.CivilizationData.Name}!", NotificationType.Warning);
                return; 
            }
            string planetName = planet.Name;
            Destroy(planet.gameObject);
            Planets[orbit][positionInOrbit] = null;
            OnPlanetRemoved?.Invoke(orbit, positionInOrbit);
            if(UnityEngine.Camera.main != null) 
                UnityEngine.Camera.main.GetComponent<Camera.CameraController2D>().ClearTarget();
                
            NotificationManager.Instance.CreateNotification($"The planet: {planetName} was deleted from the orbit {orbit + 1}", NotificationType.Info);
            ConstructionEvents.OnPlanetRemoved?.Invoke(planet);
        }

        public void BuildSpecialPlanet(STARSDataSO specialPlanetData)
        {
            GameObject starsObj = new GameObject(specialPlanetData.constructibleName);
            starsObj.transform.SetParent(this.transform); // Hijo del sistema solar
            starsObj.transform.localScale = Vector3.one * (specialPlanetData.size > 0 ? specialPlanetData.size : 1f);
            SpriteRenderer sr = starsObj.AddComponent<SpriteRenderer>();
            sr.sprite = specialPlanetData.sprite;
            if (specialPlanetData.material != null)
            {
                sr.material = specialPlanetData.material;
            }
            starsObj.AddComponent<BoxCollider2D>();
            SatelliteOrbitController orbitCtrl = starsObj.AddComponent<SatelliteOrbitController>();
            orbitCtrl.Initialize(specialPlanetData.orbitDistance, specialPlanetData.orbitSpeed, 0f);

            ConstructionEvents.OnConstructibleCreated?.Invoke(specialPlanetData);
            NotificationManager.Instance.CreateNotification($"STARS Launched: {specialPlanetData.constructibleName}", NotificationType.Info);
        }
        
        public Transform GetSunTransform()
        {
            return sunPos;
        }

        public string GetSaveId()
        {
            return "SolarSystem";
        }

        public JToken CaptureState()
        {
            JObject obj = new JObject();
            JArray orbitsArray = new JArray();

            for (int i = 0; i < Planets.Count; i++)
            {
                JArray orbitArray = new JArray();
                for (int j = 0; j < Planets[i].Count; j++)
                {
                    Planet planet = Planets[i][j];
                    if (planet == null)
                    {
                        orbitArray.Add(JValue.CreateNull());
                    }
                    else
                    {
                        orbitArray.Add(planet.CaptureState());
                    }
                }
                orbitsArray.Add(orbitArray);
            }

            obj["Planets"] = orbitsArray;
            return obj;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = (JObject)state;
            JArray orbitsArray = (JArray)obj["Planets"];

            for (int i = 0; i < orbitsArray.Count; i++)
            {
                JArray orbitArray = (JArray)orbitsArray[i];

                for (int j = 0; j < orbitArray.Count; j++)
                {
                    JToken planetToken = orbitArray[j];

                    if (planetToken.Type == JTokenType.Null)
                    {
                        Planets[i][j] = null;
                        continue;
                    }

                    JObject planetObj = (JObject)planetToken;
                    string planetName = planetObj["PlanetName"].ToString();

                    PlanetDataSO planetData = Array.Find(planetDatas,
                        pd => pd.constructibleName == planetName);

                    if (planetData == null)
                    {
                        Debug.LogError($"PlanetDataSO NOT FOUND for {planetName}");
                        continue;
                    }

                    // → RECREAR EL PLANETA
                    Planet planet = planetFactory.CreatePlanet(
                        Vector3.zero,
                        planetData,
                        transform,
                        i, j
                    );

                    // → Restaurar órbita
                    OrbitController orbitCtrl = planet.gameObject.AddComponent<OrbitController>();
                    orbitCtrl.Initialize(
                        planet,
                        (i + 1) * orbitDistanceIncrement,
                        j,
                        planetsPerOrbit[i],
                        rotationSpeed,
                        i
                    );

                    // → Restaurar estado completo del planeta
                    planet.RestoreState(planetObj);

                    Planets[i][j] = planet;
                }
            }
        }
        
        public int GetTotalResourceProductionFor(List<ResourceType> targetResources)
        {
            int grandTotalProduction = 0;
    
            foreach (var targetType in targetResources)
            {
                int totalForThisType = 0;
        
                foreach (var orbit in Planets)
                {
                    foreach (var planet in orbit)
                    {
                        if (planet == null) continue;

                        totalForThisType += planet.GetResourceProductionOfType(targetType); 
                    }
                }
                grandTotalProduction += totalForThisType;
            }
            return grandTotalProduction;
        }
    }
}