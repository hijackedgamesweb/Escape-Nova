using System;
using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
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
        
        public List<List<Planet>> Planets = new();

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

        public void AddPlanet(int orbit, int positionInOrbit, PlanetDataSO data)
        {
            if(Planets[orbit][positionInOrbit] != null && Planets[orbit][positionInOrbit].gameObject != null)
            {
                Debug.LogWarning($"There is already a planet at orbit {orbit} position {positionInOrbit}");
                return;
            }
            Planet planet = planetFactory.CreatePlanet(Vector3.zero, data, transform, orbit, positionInOrbit);

            OrbitController orbitCtrl = planet.gameObject.AddComponent<OrbitController>();
            orbitCtrl.Initialize(planet, (orbit + 1) * orbitDistanceIncrement, positionInOrbit, planetsPerOrbit[orbit], rotationSpeed, orbit);
            NotificationManager.Instance.CreateNotification($"Se ha añadido un planeta: {data.constructibleName} en la órbita {orbit + 1}", NotificationType.Info);
            Planets[orbit][positionInOrbit] = planet;
            ConstructionEvents.OnConstructibleCreated?.Invoke(data);
            ConstructionEvents.OnPlanetAdded?.Invoke(planet);
        }

        public void AddSateliteToPlanet(int orbitIndex, int positionInOrbit, SateliteDataSO sateliteDataSo)
        {
            Planet planet = Planets[orbitIndex][positionInOrbit];
            if (planet == null)
            {
                Debug.LogWarning($"No planet found at orbit {orbitIndex} position {positionInOrbit} to add a satellite.");
                return;
            }

            planet.AddSatelite(sateliteDataSo);
            ConstructionEvents.OnConstructibleCreated?.Invoke(sateliteDataSo);
        }
    }
}