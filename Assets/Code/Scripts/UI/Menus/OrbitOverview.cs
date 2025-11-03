using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using UnityEngine;
using UnityEngine.UI;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.UI.Menus.BuildingMenuPanel;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus
{
    public class OrbitOverview : MonoBehaviour
    {
        [SerializeField] private SolarSystem _solarSystem;
        [SerializeField] private Sprite _planetSlotSprite;
        [SerializeField] private Sprite _orbitCircleSprite; 
        [SerializeField] private RectTransform _orbitContainer;
        [SerializeField] private float orbitRadiusStep = 150f;
        [SerializeField] private PlanetListInitializer _planetListInitializer;
        [SerializeField] private PerfectViewScreen _perfectViewScreen;
        private List<List<GameObject>> _planetSlots = new List<List<GameObject>>();
        private void Start()
        {
            DrawOrbitsAndPlanets();
        }

        private void PlanetSlotInitializer(GameObject planetSlot, int orbitIndex, int positionInOrbit)
        {
            planetSlot.AddComponent<Button>();
            planetSlot.GetComponent<Button>().onClick.AddListener(() =>
            { 
                PlanetDataSO planetDataSo = _planetListInitializer.GetCurrentPlanetData();
                _solarSystem.AddPlanet(orbitIndex, positionInOrbit, planetDataSo);
                AddPlanetToOverview(orbitIndex, positionInOrbit, planetDataSo);
                UIManager.Instance.ShowScreen<InGameScreen>();
            });
        }
        
        private void AddPlanetToOverview(int orbitIndex, int positionInOrbit, PlanetDataSO planetDataSo)
        {
            GameObject planetSlot = _planetSlots[orbitIndex][positionInOrbit];
            Image img = planetSlot.GetComponent<Image>();
            img.sprite = planetDataSo.sprite;
            img.color = Color.white;
            planetSlot.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        
        private void DeletePlanetFromOverview(int orbitIndex, int positionInOrbit)
        {
            GameObject planetSlot = _planetSlots[orbitIndex][positionInOrbit];
            Image img = planetSlot.GetComponent<Image>();
            img.sprite = _planetSlotSprite;
            img.color = new Color(0f, 1f, 0f, 1f);
            PlanetSlotInitializer(planetSlot, orbitIndex, positionInOrbit);
        }

        private void DrawOrbitsAndPlanets()
        {
            var planetsPerOrbit = _solarSystem.planetsPerOrbit;

            int orbitIndex = 0;
            foreach (int planetsInOrbit in planetsPerOrbit)
            {
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

                _planetSlots.Add(new List<GameObject>());
                for (int i = 0; i < planetsInOrbit; i++)
                {
                    float angle = (i * (360f / planetsInOrbit)) + 90f;
                    float rad = angle * Mathf.Deg2Rad;

                    GameObject planetSlot = new GameObject($"PlanetSlot_{orbitIndex}_{i}", typeof(RectTransform), typeof(Image));
                    planetSlot.transform.SetParent(_orbitContainer, false);

                    Image img = planetSlot.GetComponent<Image>();
                    if(_solarSystem.Planets[orbitIndex][i] != null)
                    {
                        img.sprite = _solarSystem.Planets[orbitIndex][i].GetComponent<SpriteRenderer>().sprite;
                    }
                    else
                    {
                        img.sprite = _planetSlotSprite;
                        img.color = new Color(0f, 1f, 0f, 1f);
                        PlanetSlotInitializer(planetSlot, orbitIndex, i);
                    }
                    img.SetNativeSize();
                    img.transform.localScale = Vector3.one * 0.3f;

                    Vector3 newPos = new Vector3(orbitRadius * Mathf.Cos(rad), orbitRadius * Mathf.Sin(rad), 0f);
                    planetSlot.GetComponent<RectTransform>().anchoredPosition = newPos;
                    _planetSlots[orbitIndex].Add(planetSlot);
                }

                orbitIndex++;
            }
        }
    }
}
