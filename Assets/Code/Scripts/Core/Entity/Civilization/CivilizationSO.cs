using System;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.Core.Entity.Civilization
{
    [Serializable]
    public enum AIType
    {
        TestController,
        AkkiBehaviour,
        HalxiBehaviour,
        MippipBehaviour,
        SkulgBehaviour,
        HandoullBehaviour
    }
    public class CivilizationSO : EntitySO, IAstrariumEntry
    {

        [Header("Civilization Info")]
        [TextArea(3, 10)]
        public string civilizationDescription;
        public Sprite civilizationIcon;
        public Sprite civilizationFlag;
        
        [Header("Civilization Behaviour")]
        public AIType aiController;
        
        [Header("Civilization Attributes")]
        public float angerTolerance;
        
        [Header("Civilization Leader")]
        public Sprite leaderPortrait;
        
        [Header("Starting Resources")]
        public List<ResourceData> startingResources;
        public int[] startingResourceAmounts;
        public InventoryData startingInventory;
        
        [Header("Civilization Prefs")]
        public PlanetDataSO preferredPlanet;

        // --- INTERFAZ ---
        public string GetAstrariumID() => $"civ_{civName.ToLower().Replace(" ", "_")}";
        public string GetDisplayName() => civName;
        public string GetDescription() => civilizationDescription;
        public Sprite GetIcon() => civilizationIcon; 
        public AstrariumCategory GetCategory() => AstrariumCategory.Species;
        public GameObject Get3DModel() => null;
    }
}