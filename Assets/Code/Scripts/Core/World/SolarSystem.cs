using System;
using System.Collections.Generic;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.Factory;
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
        }

        public void AddPlanet(int orbit, int positionInOrbit, PlanetDataSO data)
        {
            if(Planets[orbit][positionInOrbit] != null && Planets[orbit][positionInOrbit].gameObject != null)
            {
                Debug.LogWarning($"There is already a planet at orbit {orbit} position {positionInOrbit}");
                return;
            }
            Planet planet = planetFactory.CreatePlanet(Vector3.zero, data, transform);

            OrbitController orbitCtrl = planet.gameObject.AddComponent<OrbitController>();
            orbitCtrl.Initialize(planet, (orbit + 1) * orbitDistanceIncrement, positionInOrbit, planetsPerOrbit[orbit], rotationSpeed, orbit);
            Planets[orbit][positionInOrbit] = planet;
        }
        
    }
}