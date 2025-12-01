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
            
            WeightedFusionFactor disgustedFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["IndependencyCurve"], _curveFactors["IndifirentCurve"], _curveFactors["DistrustCurve"]);
            disgustedFusion.Weights = new float[]{ 0.15f, 0.35f, 0.35f, 0.15f };
            
            WeightedFusionFactor progressiveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["InterestCurve"]);
            progressiveFusion.Weights = new float[]{ 0.3f, 0.7f };
            
            WeightedFusionFactor neededFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["DependencyCurve"], _curveFactors["FaithCurve"]);
            neededFusion.Weights = new float[]{ 0.1f, 0.6f, 0.3f };
            
            WeightedFusionFactor commerceFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["IndependencyCurve"], _curveFactors["InterestCurve"]);
            commerceFusion.Weights = new float[]{ 0.2f, 0.8f };
            
            WeightedFusionFactor loveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            loveFusion.Weights = new float[]{ 0.1f, 0.6f, 0.3f };
            
            WeightedFusionFactor negotiationFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            negotiationFusion.Weights = new float[]{ 0.6f, 0.4f };
            
            
            UtilityAction runawayAction = UtilitySystem.CreateAction(disgustedFusion, _actions["SetDisgusted"]);
            UtilityAction proposeInvestigationAction = UtilitySystem.CreateAction(progressiveFusion, _actions["SetProgressive"]);
            UtilityAction seekHelpAction = UtilitySystem.CreateAction(neededFusion, _actions["SetNeeded"]);
            UtilityAction increaseTradeAction = UtilitySystem.CreateAction(commerceFusion, _actions["SetCommerce"]);
            UtilityAction proposeAllianceAction = UtilitySystem.CreateAction(loveFusion, _actions["SetLove"]);
            UtilityAction exchangeTreatyAction = UtilitySystem.CreateAction(negotiationFusion, _actions["SetNegotiation"]);
            
        }

    }
}