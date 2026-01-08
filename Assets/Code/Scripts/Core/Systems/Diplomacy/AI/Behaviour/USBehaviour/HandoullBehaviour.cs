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
    public class HandoullBehaviour : BaseBehaviour
    {
        public HandoullBehaviour(Entity.Civilization.Civilization civ, CommandInvoker invoker) : base(civ, invoker)
        {
        }
        
        protected override void InitializeUtilitySystem()
        {
            
            //Beligerantes
            WeightedFusionFactor enemyFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["InterestCurve"]);
            enemyFusion.Weights = new float[]{ 0.75f, 0.25f };
            
            //Necesitados
            WeightedFusionFactor neededFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            neededFusion.Weights = new float[]{ 0.7f, 0.3f };
            
            //Asqueados
            WeightedFusionFactor loveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["IndiferentCurve"], _curveFactors["DistrustCurve"]);
            loveFusion.Weights = new float[]{ 0.8f, 0.2f };
            
            
            
            //Asqueados
            UtilitySystem.CreateAction(loveFusion, _actions["Runaway"]);
            
            //Beligerantes
            UtilitySystem.CreateAction(enemyFusion, _actions["DeclareWar"]);
            
            //Necesitados
            UtilitySystem.CreateAction(neededFusion, _actions["SeekHelp"]);
            
        }

    }
}