using System;
using Code.Scripts.Camera;
using Code.Scripts.Core.Managers.Interfaces;
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
        private PlanetDataSO _planetData;
        public PlanetDataSO PlanetData => _planetData;
        
        PlanetStateManager _stateManager;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }


        public void InitializePlanet(PlanetDataSO data)
        {
            _planetData = data;
            _spriteRenderer.sprite = data.sprite;
            this.name = data.constructibleName;
            this.transform.localScale = Vector3.one * data.size;
            
            
            var gametTime = ServiceLocator.GetService<IGameTime>();
            _stateManager = new PlanetStateManager(gametTime);
            _stateManager.SetState(new BuildingState(_planetData, gametTime));
        }
        
        

        public void OnMouseDown()
        {
            Debug.Log("Planet clicked: " + this.name);
            UnityEngine.Camera.main.GetComponent<CameraController2D>().SetTarget(this.transform);
        }

    }
}