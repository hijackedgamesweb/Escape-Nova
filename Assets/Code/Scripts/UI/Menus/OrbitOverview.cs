using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using UnityEngine;
using UnityEngine.UI;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.UI.Common;
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
        [SerializeField] private SateliteListInitializer _sateliteListInitializer;
        [SerializeField] private PerfectViewScreen _perfectViewScreen;
        private List<List<GameObject>> _planetSlots = new List<List<GameObject>>();
        
        private ConstructionType currentConstructionType = ConstructionType.Planet;
        private void Start()
        {
            DrawOrbit(0);
            ConstructionPanelManager.OnConstructionTypeChanged += OnConstructionTypeChanged;
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
                    _solarSystem.AddPlanet(orbitIndex, positionInOrbit, planetDataSo);
                    AddPlanetToOverview(orbitIndex, positionInOrbit, planetDataSo);
                    UIManager.Instance.ShowScreen<InGameScreen>();
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
                    _solarSystem.AddSateliteToPlanet(orbitIndex, positionInOrbit, sateliteDataSo);
                    UIManager.Instance.ShowScreen<InGameScreen>();
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
                Debug.Log($"Nueva órbita añadida: {nextOrbitIndex}");
            }
            else
            {
                Debug.Log("No hay más órbitas disponibles en el SolarSystem.");
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

            // Seguridad: evitar índice fuera de rango
            if (orbitIndex < 0 || orbitIndex >= planetsPerOrbit.Length)
            {
                Debug.LogWarning($"Orbit index {orbitIndex} fuera de rango.");
                return;
            }

            float orbitRadius = orbitRadiusStep * (orbitIndex + 1);

            // Crear el círculo de la órbita
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

            // Crear los planet slots
            int planetsInOrbit = planetsPerOrbit[orbitIndex];
            _planetSlots.Add(new List<GameObject>());

            for (int i = 0; i < planetsInOrbit; i++)
            {
                float angle = (i * (360f / planetsInOrbit)) + 90f;
                float rad = angle * Mathf.Deg2Rad;

                GameObject planetSlot = new GameObject($"PlanetSlot_{orbitIndex}_{i}", typeof(RectTransform), typeof(Image));
                planetSlot.transform.SetParent(_orbitContainer, false);

                Image img = planetSlot.GetComponent<Image>();
                if (_solarSystem.Planets[orbitIndex][i] != null)
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
                img.transform.localScale = Vector3.one * 0.1f;

                Vector3 newPos = new Vector3(orbitRadius * Mathf.Cos(rad), orbitRadius * Mathf.Sin(rad), 0f);
                planetSlot.GetComponent<RectTransform>().anchoredPosition = newPos;

                _planetSlots[orbitIndex].Add(planetSlot);
            }
        }
    }
}
