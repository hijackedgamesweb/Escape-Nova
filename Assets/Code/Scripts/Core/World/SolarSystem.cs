using System;
using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Scripts.Core.World
{
    public class SolarSystem : MonoBehaviour
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
        
        public List<List<Planet>> Planets = new();
        
        public event Action<int, int> OnPlanetRemoved;


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
        }

        public void AddSateliteToPlanet(int orbitIndex, int positionInOrbit, SateliteDataSO sateliteDataSo)
        {
            Planet planet = Planets[orbitIndex][positionInOrbit];
            if (planet == null)
            {
                return;
            }

            planet.AddSatelite(sateliteDataSo);
            ConstructionEvents.OnConstructibleCreated?.Invoke(sateliteDataSo);
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
            string planetName = planet.Name;
            Destroy(planet.gameObject);
            Planets[orbit][positionInOrbit] = null;
            OnPlanetRemoved?.Invoke(orbit, positionInOrbit);
            UnityEngine.Camera.main.GetComponent<Camera.CameraController2D>().ClearTarget();
            NotificationManager.Instance.CreateNotification($"The planet: {planetName} was deleted from the orbit {orbit + 1}", NotificationType.Info);
            ConstructionEvents.OnPlanetRemoved?.Invoke(planet);
        }

        public void BuildSpecialPlanet(STARSDataSO specialPlanetData)
        {
            //NotificationManager.Instance.CreateNotification($"Se ha construido: {specialPlanetData.constructibleName}!", NotificationType.Info);
            ConstructionEvents.OnConstructibleCreated?.Invoke(specialPlanetData);
        }
    }
}