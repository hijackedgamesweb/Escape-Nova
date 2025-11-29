using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using UnityEngine;

namespace Code.Scripts.Core.Entity.Civilization
{
    public enum AIType
    {
        TestController,
        AkkiBehaviour,
        HalxiBehaviour,
        MippipBehaviour,
        SkulgBehaviour,
        HandoullBehaviour
    }
    public class CivilizationSO : EntitySO
    {
        [Header("Civilization Info")]
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
        public InventoryData startingInventory;
        
    }
}