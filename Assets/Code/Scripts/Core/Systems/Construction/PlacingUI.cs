using System;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class PlacingUI : MonoBehaviour
{
    //VARIABLES
    
    [SerializeField] private GameObject ConstructionUI;

    public ConstructibleDataSO entityToBuild;
    
    //METODOS
    
    public void PlacingLocationPressed(int orbitIndex, int positionIndex)
    {
        if (entityToBuild == null)
        {
            return;
        }

        var solarSystem = Code.Scripts.Patterns.ServiceLocator.ServiceLocator.GetService<Code.Scripts.Core.World.SolarSystem>();

        if (entityToBuild is PlanetDataSO)
        {
            solarSystem.AddPlanet(orbitIndex, positionIndex, entityToBuild as PlanetDataSO);
        }
        else if (entityToBuild is SateliteDataSO)
        {
            solarSystem.AddSateliteToPlanet(orbitIndex, positionIndex, entityToBuild as SateliteDataSO);
        }

        entityToBuild = null;
        
        ConstructionUI.SetActive(true);
        gameObject.SetActive(false);
    }
}