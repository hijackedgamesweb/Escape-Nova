using System.Collections.Generic;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities
{
    public class Satelite : ISaveable
    {
        private SpriteRenderer _spriteRenderer;
        public int TimeToBuild { get; private set; }
        public Planet Planet { get; set; }
        public string Name { get; private set; }
        public SateliteDataSO SateliteData { get; private set; }

        public void InitializeSatelite(SateliteDataSO sateliteDataSo, Planet planet)
        {
            SateliteData = sateliteDataSo;
            Name = sateliteDataSo.constructibleName;
            TimeToBuild = sateliteDataSo.timeToBuild;
            Planet = planet;
        }

        public string GetSaveId()
        {
            return $"{Planet.GetSaveId()}_Satelite_{Name}";
        }

        public JToken CaptureState()
        {
            var state = new JObject
            {
                ["Name"] = Name,
                ["TimeToBuild"] = TimeToBuild,
                ["SateliteData"] = SateliteData != null ? JToken.FromObject(SateliteData.name) : JValue.CreateNull()
            };
            return state;
        }

        public void RestoreState(JToken state)
        {
            Name = state["Name"].ToObject<string>();
            TimeToBuild = state["TimeToBuild"].ToObject<int>();
            var sateliteDataName = state["SateliteData"]?.ToObject<string>();
            if (!string.IsNullOrEmpty(sateliteDataName))
            {
                var sateliteDataArray = Resources.LoadAll<SateliteDataSO>("ScriptableObjects/Satelites");
                SateliteData = System.Array.Find(sateliteDataArray, data => data.name == sateliteDataName);
            }
        }
    }
}