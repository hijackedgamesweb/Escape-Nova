using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using Code.Scripts.Player;

namespace Code.Scripts.Core.Entity.Player
{
    public class Player : Entity
    {
        private StorageSystem _storageSystem;
        private PlayerData _playerData;
        private PlayerState _playerState;
        
        public Player(CommandInvoker invoker, PlayerSO playerSO, List<ResourceData> startingResources, InventoryData startingInventory) : base(invoker, playerSO)
        {
            _storageSystem = new StorageSystem(startingResources, startingInventory);
            
            _invoker = invoker;
            _playerData = new PlayerData(playerSO);
            _playerState = new PlayerState(playerSO);
        }
         
        public void AddCommand(ICommand command)
        {
            _invoker.ExecuteCommand(command);
        }
    }
}