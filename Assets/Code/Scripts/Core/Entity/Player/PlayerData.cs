using Code.Scripts.Core.Entity;
using Code.Scripts.Core.Entity.Player;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Newtonsoft.Json.Linq;

namespace Code.Scripts.Player
{
    public class PlayerData : EntityData, ISaveable
    {
        public PlayerData(PlayerSO playerSo) : base(playerSo)
        {
            
        }

        public string GetSaveId()
        {
            return "PlayerData";
        }

        public JToken CaptureState()
        {
            JObject state = new JObject
            {
                ["Name"] = Name,
                ["LeaderName"] = LeaderName,
                ["AngerTolerance"] = AngerTolerance
            };
            return state;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = (JObject)state;
            Name = obj["Name"].ToObject<string>();
            LeaderName = obj["LeaderName"].ToObject<string>();
            AngerTolerance = obj["AngerTolerance"].ToObject<float>();
        }
    }
}