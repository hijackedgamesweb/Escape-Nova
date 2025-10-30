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
        [SerializeField] private int[] planetsPerOrbit;
        [SerializeField] private float orbitDistanceIncrement;
        [SerializeField] private float rotationSpeed = 20f;

        [Header("Planet Factory")]
        [SerializeField] private PlanetFactory planetFactory;
        [SerializeField] private PlanetDataSO[] planetDatas;
        
        private List<List<Planet>> _planets = new();

        private void Awake()
        {
            for (int i = 0; i < planetsPerOrbit.Length; i++)
            {
                _planets.Add(new List<Planet>());
            }
            GeneratePlanets();
        }

        private void GeneratePlanets()
        {
            int orbitIndex = 0;
            foreach (int planetsInOrbit in planetsPerOrbit)
            {
                for (int i = 0; i < planetsInOrbit; i++)
                {
                    // Seleccionamos un PlanetData (por ejemplo al azar o en orden)
                    PlanetDataSO data = planetDatas[Random.Range(0, planetDatas.Length)];

                    Planet planet = planetFactory.CreatePlanet(Vector3.zero, data, transform);

                    // Añadir controlador de órbita
                    OrbitController orbitCtrl = planet.gameObject.AddComponent<OrbitController>();
                    orbitCtrl.Initialize(planet, (orbitIndex + 1) * orbitDistanceIncrement, i, planetsInOrbit, rotationSpeed);

                    _planets[orbitIndex].Add(planet);
                }
                orbitIndex++;
            }
        }
    }
}