using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Entity.Civilization
{
    public class CivilizationData : EntityData, ISaveable
    {
        public string CivilizationDescription;
        public Sprite CivilizationIcon;
        public Sprite CivilizationFlag;
        
        
        public Sprite LeaderPortrait;

        public float AngerTolerance;
        public PlanetDataSO HomePlanetData;

        public CivilizationData(CivilizationSO entitySO) : base(entitySO)
        {
            CivilizationDescription = entitySO.civilizationDescription;
            CivilizationIcon = entitySO.civilizationIcon;
            CivilizationFlag = entitySO.civilizationFlag;
            LeaderPortrait = entitySO.leaderPortrait;
            AngerTolerance = entitySO.angerTolerance;
            HomePlanetData = entitySO.preferredPlanet;
        }
        
        public CivilizationData() : base()
        {
            CivilizationDescription = "";
            CivilizationIcon = null;
            CivilizationFlag = null;
            LeaderPortrait = null;
            AngerTolerance = 0f;
        }

        public string GetSaveId()
        {
            return $"CivilizationData_{Name}";
        }

        public JToken CaptureState()
        {
            JObject obj = new JObject
            {
                ["Name"] = Name,
                ["CivilizationDescription"] = CivilizationDescription,
                ["AngerTolerance"] = AngerTolerance
            };
            return obj;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = state as JObject;
            Name = obj["Name"].ToObject<string>();
            CivilizationDescription = obj["CivilizationDescription"].ToObject<string>();
            AngerTolerance = obj["AngerTolerance"].ToObject<float>();
        }
    }
}