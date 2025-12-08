using System.Collections.Generic;
using Code.Scripts.Core.Systems.Storage;
using UnityEngine;

namespace Code.Scripts.Core.Entity
{
    [CreateAssetMenu(fileName = "Entity Item Preferences", menuName = "ScriptableObjects/Entity/Entity Item Preferences SO", order = 1)]
    public class EntityItemPreferencesSO : ScriptableObject
    {
        public float stoneWorth = 1f;
        public float metalWorth = 2f;
        public float fireWorth = 3f;
        public float iceWorth = 3f;
        public float sandWorth = 1.5f;
        
        public List<InventoryItem> tier0Items;
        public List<InventoryItem> tier2Items;
        public List<InventoryItem> tier3Items;
        public List<InventoryItem> tier4Items;
        public List<InventoryItem> tier5Items;
        
    }
}