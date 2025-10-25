using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class Player
    {
        private StorageSystem _storageSystem;
        private CommandInvoker _invoker;
        
        public Player(CommandInvoker invoker, List<ResourceData> startingResources)
        {
            Debug.Log("Player Created");
            _storageSystem = new StorageSystem(startingResources);
            _invoker = invoker;
        }
        
        
        public void AddCommand(ICommand command)
        {
            _invoker.ExecuteCommand(command);
        }
    }
}