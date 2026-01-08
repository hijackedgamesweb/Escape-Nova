using System;
using System.Collections.Generic;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UtilitySystems;
using Code.Scripts.Core.Systems.Civilization.AI.Behaviour;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Diplomacy.ScriptableObjects;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour
{
    public class MippipBehaviour : BaseBehaviour
    {
        public MippipBehaviour(Entity.Civilization.Civilization civ, CommandInvoker invoker) : base(civ, invoker)
        {
        }
        
        protected override void InitializeUtilitySystem()
        {
            base.InitializeUtilitySystem();
            
            //Asqueados
            WeightedFusionFactor enemyFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["IndependencyCurve"]);
            enemyFusion.Weights = new float[]{ 0.3f, 0.7f };
            
            //Necesitados
            WeightedFusionFactor neededFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["DependencyCurve"], _curveFactors["FaithCurve"]);
            neededFusion.Weights = new float[]{ 0.8f, 0.2f };
            
            //Generosos
            WeightedFusionFactor loveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["IndependencyCurve"]);
            loveFusion.Weights = new float[]{ 0.8f, 0.2f };
            
            
            //Generosos
            UtilitySystem.CreateAction(loveFusion, _actions["OfferGift"]);
            
            //Asqueados
            UtilitySystem.CreateAction(enemyFusion, _actions["Runaway"]);
            
            //Necesitados
            UtilitySystem.CreateAction(neededFusion, _actions["SeekHelp"]);
            
        }

    }
}