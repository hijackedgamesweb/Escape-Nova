using System;
using System.Collections.Generic;
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
        public List<ResourceType> ProducibleResources { get; private set; }
        public int TimeToBuild { get; private set; }
        public int OrbitIndex { get; set; }
        public int PlanetIndex { get; set; }
        public string Name { get; private set; }
        
        PlanetStateManager _stateManager;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }


        public void InitializePlanet(PlanetDataSO data, int orbit, int positionInOrbit)
        {
            _spriteRenderer.sprite = data.sprite;
            Name = data.constructibleName;
            this.transform.localScale = Vector3.one * data.size;
            ResourcePerCycle = (int[]) data.resourcePerCycle.Clone();
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

        public void IncreaseProductionPerCycleOfAllResources(float percentage)
        {
            for (int i = 0; i < ResourcePerCycle.Length; i++)
            {
                ResourcePerCycle[i] = Mathf.CeilToInt(ResourcePerCycle[i] * (1 + percentage / 100f));
            }
        }
        

        public void OnMouseDown()
        {
            Debug.Log("Planet clicked: " + this.name);
            UnityEngine.Camera.main.GetComponent<CameraController2D>().SetTarget(this.transform);
        }

    }
}