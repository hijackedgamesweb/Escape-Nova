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
    public class AkkiBehaviour : BaseBehaviour
    {
        public AkkiBehaviour(Entity.Civilization.Civilization civ, CommandInvoker invoker) : base(civ, invoker)
        {
        }
        
        protected override void InitializeUtilitySystem()
        {
            base.InitializeUtilitySystem();
            
            WeightedFusionFactor peaceFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            peaceFusion.Weights = new float[]{ 0.1f, 0.7f, 0.2f };
            
            WeightedFusionFactor loveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            loveFusion.Weights = new float[]{ 0.1f, 0.8f, 0.1f };
            
            WeightedFusionFactor disgustedFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["IndependencyCurve"], _curveFactors["IndifirentCurve"], _curveFactors["DistrustCurve"]);
            disgustedFusion.Weights = new float[]{ 0.1f, 0.4f, 0.4f, 0.1f };
            
            WeightedFusionFactor neededFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["DependencyCurve"], _curveFactors["FaithCurve"]);
            neededFusion.Weights = new float[]{ 0.1f, 0.3f, 0.6f };
            
            WeightedFusionFactor progressiveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["InterestCurve"]);
            progressiveFusion.Weights = new float[]{ 0.2f, 0.8f };
            
            WeightedFusionFactor commerceFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["IndependencyCurve"], _curveFactors["InterestCurve"]);
            commerceFusion.Weights = new float[]{ 0.5f, 0.5f };
            
            WeightedFusionFactor belligerentFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["InterestCurve"], _curveFactors["DistrustCurve"]);
            belligerentFusion.Weights = new float[]{ 0.1f, 0.2f, 0.7f };
            
            WeightedFusionFactor negotiationFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            negotiationFusion.Weights = new float[]{ 0.7f, 0.3f };
            
            
            UtilityAction runawayAction = UtilitySystem.CreateAction(disgustedFusion, _actions["Runaway"], true);
            UtilityAction offerPeaceAction = UtilitySystem.CreateAction(peaceFusion, _actions["OfferPeace"]);
            UtilityAction declareWarAction = UtilitySystem.CreateAction(belligerentFusion, _actions["DeclareWar"]);
            UtilityAction proposeAllianceAction = UtilitySystem.CreateAction(loveFusion, _actions["ProposeMarriage"]);
            UtilityAction seekHelpAction = UtilitySystem.CreateAction(neededFusion, _actions["SeekHelp"]);
            UtilityAction increaseTradeAction = UtilitySystem.CreateAction(commerceFusion, _actions["IncreaseTrade"], true);
            UtilityAction proposeInvestigationAction = UtilitySystem.CreateAction(progressiveFusion, _actions["ProposeInvestigation"]);
            UtilityAction exchangeTreatyAction = UtilitySystem.CreateAction(negotiationFusion, _actions["ExchangeTreaty"]);
            
        }

    }
}