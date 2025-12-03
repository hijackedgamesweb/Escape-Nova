using System.Collections.Generic;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using Code.Scripts.Player;
using Newtonsoft.Json.Linq;

namespace Code.Scripts.Core.Entity.Player
{
    public class Player : Entity, ISaveable
    {
        private string ID => "Player";
        private PlayerData _playerData;
        private PlayerState _playerState;
        
        public Player(CommandInvoker invoker, PlayerSO playerSO, StorageSystem storageSystem) : base(invoker, playerSO, storageSystem)
        {
            StorageSystem = storageSystem;
            ItemPreferences = new EntityItemPreferences(playerSO.itemPreferences);
            
            _invoker = invoker;
            _playerData = new PlayerData(playerSO);
            _playerState = new PlayerState(playerSO);
        }
         
        public void AddCommand(ICommand command)
        {
            _invoker.ExecuteCommand(command);
        }

        public string GetSaveId()
        {
            return ID;
        }

        public JToken CaptureState()
        {
            JObject state = new JObject
            {
                ["playerData"] = _playerData.CaptureState(),
                ["storageSystem"] = StorageSystem.CaptureState()
            };
            return state;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = (JObject)state;
            _playerData.RestoreState(obj["playerData"]);
            StorageSystem.RestoreState(obj["storageSystem"]);
        }
    }
}