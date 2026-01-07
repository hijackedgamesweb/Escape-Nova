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
    public class SkulgBehaviour : BaseBehaviour
    {
        public SkulgBehaviour(Entity.Civilization.Civilization civ, CommandInvoker invoker) : base(civ, invoker)
        {
        }
        
        protected override void InitializeUtilitySystem()
        {
            base.InitializeUtilitySystem();
            
            //Ofendidos
            WeightedFusionFactor disgustedFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["InterestCurve"]);
            disgustedFusion.Weights = new float[]{ 0.2f, 0.8f };
            
            //Beligerantes
            WeightedFusionFactor enemyFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["InterestCurve"]);
            enemyFusion.Weights = new float[]{ 0.6f, 0.4f };
            
            //Necesitados
            WeightedFusionFactor neededFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["DependencyCurve"], _curveFactors["FaithCurve"]);
            neededFusion.Weights = new float[]{ 0.7f, 0.3f };
            
            //Generosos
            WeightedFusionFactor loveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["FaithCurve"]);
            loveFusion.Weights = new float[]{ 0.5f, 0.5f };
            
            
            //Ofendidos
            UtilitySystem.CreateAction(disgustedFusion, _actions["DemandTribute"]);
            
            //Generosos
            UtilitySystem.CreateAction(loveFusion, _actions["OfferGift"]);
            
            //Beligerantes
            UtilitySystem.CreateAction(enemyFusion, _actions["DeclareWar"]);
            
            //Necesitados
            UtilitySystem.CreateAction(neededFusion, _actions["SeekHelp"]);
            
        }

    }
}