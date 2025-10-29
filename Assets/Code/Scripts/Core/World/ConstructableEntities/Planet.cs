using System;
using Code.Scripts.Camera;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.Core.World.ConstructableEntities
{
    public class Planet : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private PlanetDataSO _planetData;
        public PlanetDataSO PlanetData => _planetData;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void InitializePlanet(PlanetDataSO data)
        {
            _planetData = data;
            _spriteRenderer.sprite = data.sprite;
            this.name = data.planetName;
            this.transform.localScale = Vector3.one * data.size;
        }

        public void OnMouseDown()
        {
            Debug.Log("Planet clicked: " + this.name);
            UnityEngine.Camera.main.GetComponent<CameraController2D>().SetTarget(this.transform);
        }
    }
}