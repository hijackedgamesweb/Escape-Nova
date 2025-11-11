using System;
using System.Collections.Generic;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UtilitySystems;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour
{
    public class BaseBehaviour : IAIController
    {
        protected UtilitySystem UtilitySystem = new UtilitySystem();
        protected Entity.Civilization.Civilization _civilization;
        protected WorldContext _worldContext;
        protected CommandInvoker _invoker;
        
        protected Dictionary<string, CurveFactor> _curveFactors = new Dictionary<string, CurveFactor>();
        protected Dictionary<string, FunctionalAction> _actions = new Dictionary<string, FunctionalAction>();
        
        public BaseBehaviour(Entity.Civilization.Civilization civ, CommandInvoker invoker)
        {
            _civilization = civ;
            _invoker = invoker;
            InitializeUtilitySystem();
            UtilitySystem.Start();
        }

        protected virtual void InitializeUtilitySystem()
        {
            VariableFactor vFriendliness = UtilitySystem.CreateVariable(() => _civilization.CivilizationState.FriendlinessLevel, 0f, 1f);
            VariableFactor vDependability = UtilitySystem.CreateVariable(() => _civilization.CivilizationState.DependencyLevel, 0f, 1f);
            VariableFactor vInterest = UtilitySystem.CreateVariable(() => _civilization.CivilizationState.InterestLevel, 0f, 1f);
            VariableFactor vTrustworthiness = UtilitySystem.CreateVariable(() => _civilization.CivilizationState.TrustLevel, 0f, 1f);
            
            LinearCurveFactor friendshipCurve = UtilitySystem.CreateCurve<LinearCurveFactor>(vFriendliness);
            friendshipCurve.Slope = 1f;
            friendshipCurve.YIntercept = 0f;
            _curveFactors["FriendshipCurve"] = friendshipCurve;
            
            LinearCurveFactor enemyCurve = UtilitySystem.CreateCurve<LinearCurveFactor>(vFriendliness);
            enemyCurve.Slope = -1f;
            enemyCurve.YIntercept = 1f;
            _curveFactors["EnemyCurve"] = enemyCurve;
            
            CustomCurveFactor dependencyCurve = UtilitySystem.CreateCurve<CustomCurveFactor>(vDependability);
            dependencyCurve.Function = (x) =>
            {
                if (x <= 0.6f)
                    return 0f;
                float X = x * 100f;
                double numerator = Math.Exp(0.1 * (X - 60)) - 1;
                double denominator = Math.Exp(4) - 1;
                return (float)(numerator / denominator);
            };
            _curveFactors["DependencyCurve"] = dependencyCurve;
            
            CustomCurveFactor independencyCurve = UtilitySystem.CreateCurve<CustomCurveFactor>(vDependability);
            independencyCurve.Function = (x) =>
            {
                if (x <= 0.6f)
                    return 1f;
                float X = x * 100f;
                double numerator = Math.Exp(-0.1 * (X - 60)) - Math.Exp(-4);
                double denominator = 1 - Math.Exp(-4);
                return (float)(numerator / denominator);
            };
            _curveFactors["IndependencyCurve"] = independencyCurve;
            
            PointedCurveFactor interestCurve = UtilitySystem.CreateCurve<PointedCurveFactor>(vInterest);
            interestCurve.Points = new List<CurvePoint>
            {
                new CurvePoint(0f, 0f),
                new CurvePoint(0.5f, 0f),
                new CurvePoint(0.8f, 1f),
                new CurvePoint(1f, 1f)
            };
            _curveFactors["InterestCurve"] = interestCurve;
            
            PointedCurveFactor indifirentCurve = UtilitySystem.CreateCurve<PointedCurveFactor>(vInterest);
            indifirentCurve.Points = new List<CurvePoint>
            {
                new CurvePoint(0f, 1f),
                new CurvePoint(0.5f, 1f),
                new CurvePoint(0.8f, 0f),
                new CurvePoint(1f, 0f)
            };
            _curveFactors["IndifirentCurve"] = indifirentCurve;
            
            CustomCurveFactor distrustCurve = UtilitySystem.CreateCurve<CustomCurveFactor>(vTrustworthiness);
            distrustCurve.Function = (x) =>
            {
                if (x <= 0.8f)
                    return (float)(1 - Math.Pow((x/80), 2));
                return (float)(0.000625f*Math.Pow((x - 80), 2));
            };
            _curveFactors["DistrustCurve"] = distrustCurve;
            
            CustomCurveFactor faithCurve = UtilitySystem.CreateCurve<CustomCurveFactor>(vTrustworthiness);
            faithCurve.Function = (x) =>
            {
                if (x <= 0.8f)
                    return (float)Math.Pow((x/80), 2);
                return (float)(1 - 0.001875f*Math.Pow((x - 80), 2));
            };
            _curveFactors["FaithCurve"] = faithCurve;
            
             FunctionalAction offerPeace = new FunctionalAction(
                
                () => Debug.Log("Offer Peace"),
                () =>
                {
                    Debug.Log("Offering Peace");
                    return Status.Success;
                },
                () => Debug.Log("Offered Peace")
            );
             _actions["OfferPeace"] = offerPeace;
            
            FunctionalAction declareWar = new FunctionalAction(
                
                () => Debug.Log("Declare War"),
                () =>
                {
                    Debug.Log("Declaring War");
                    return Status.Success;
                },
                () => Debug.Log("Declared War")
            );
            _actions["DeclareWar"] = declareWar;
            
            FunctionalAction proposeAlliance = new FunctionalAction(
                
                () => Debug.Log("Propose Alliance"),
                () =>
                {
                    Debug.Log("Proposing Alliance");
                    return Status.Success;
                },
                () => Debug.Log("Proposed Alliance")
            );
            _actions["ProposeAlliance"] = proposeAlliance;
            
            
            FunctionalAction proposeMarriage = new FunctionalAction(
                
                () => Debug.Log("Propose Marriage"),
                () =>
                {
                    Debug.Log("Proposing Marriage");
                    return Status.Success;
                },
                () => Debug.Log("Proposed Marriage")
            );
            _actions["ProposeMarriage"] = proposeMarriage;
            
            FunctionalAction seekHelp = new FunctionalAction(
                
                () => Debug.Log("Seek Help"),
                () =>
                {
                    Debug.Log("Seeking Help");
                    return Status.Success;
                },
                () => Debug.Log("Sought Help")
            );
            _actions["SeekHelp"] = seekHelp;
            
            FunctionalAction increaseTrade = new FunctionalAction(
                
                () => Debug.Log("Increase Trade"),
                () =>
                {
                    Debug.Log("Increasing Trade");
                    _civilization.CivilizationState.FriendlinessLevel = 1f;
                    return Status.Success;
                },
                () => Debug.Log("Increased Trade")
            );
            _actions["IncreaseTrade"] = increaseTrade;
            
            FunctionalAction runaway = new FunctionalAction(
                
                () => Debug.Log("Run Away"),
                () =>
                {
                    Debug.Log("Running Away");
                    return Status.Success;
                },
                () => Debug.Log("Ran Away")
            );
            _actions["Runaway"] = runaway;
            
            FunctionalAction proposeInvestigation = new FunctionalAction(
                
                () => Debug.Log("Propose Investigation"),
                () =>
                {
                    Debug.Log("Proposing Investigation");
                    return Status.Success;
                },
                () => Debug.Log("Proposed Investigation")
            );
            _actions["ProposeInvestigation"] = proposeInvestigation;
            
            FunctionalAction exchangeTreaty = new FunctionalAction(
                
                () => Debug.Log("Exchange Treaty"),
                () =>
                {
                    Debug.Log("Exchanging Treaty");
                    return Status.Success;
                },
                () => Debug.Log("Exchanged Treaty")
            );
            _actions["ExchangeTreaty"] = exchangeTreaty;
            
            FunctionalAction giveAid = new FunctionalAction(
                
                () => Debug.Log("Give Aid"),
                () =>
                {
                    Debug.Log("Giving Aid");
                    return Status.Success;
                },
                () => Debug.Log("Gave Aid")
            );
            _actions["GiveAid"] = giveAid;
        }
        

        public void UpdateAI(WorldContext context)
        {
            _worldContext = context;
            if((int) context.CurrentTurn % 5 == 0)    
                UtilitySystem.Update();
        }

        public void UpdateAI(WorldContext context, ICommand command)
        {
        }

        public void SetCommandInvoker(CommandInvoker invoker)
        {
            _invoker = invoker;
        }
    }
}