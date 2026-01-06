using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UtilitySystems;
using Code.Scripts.Core.Managers;
using BehaviourAPI.BehaviourTrees;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.UI;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Action = System.Action;
using Object = UnityEngine.Object;

namespace Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour
{
    public class BaseBehaviour : IAIController
    {
        protected UtilitySystem UtilitySystem = new UtilitySystem();
        protected Entity.Civilization.Civilization _civilization;
        protected CommandInvoker _invoker;
        
        protected Dictionary<string, CurveFactor> _curveFactors = new Dictionary<string, CurveFactor>();
        protected Dictionary<string, FunctionalAction> _actions = new Dictionary<string, FunctionalAction>();
        
        private BehaviourTree _warTree;
        private Node _rootNode;
        private int _warHealth = 100;
        public bool _isAtWarWithPlayer { get; private set; } = false;
        private float _waitTimer = 0f;
        
        private int _enemySimulatedHealth = 100; 

        private const int COST_FIRE_STRIKE = 300;   
        private const int GATHER_AMOUNT = 30;
        private const string ITEM_FIRE_STRIKE = "Fire Strike";
        private const string RES_MAGMAVITE = "Magmavite";
        
        private const ResourceType TYPE_MAGMAVITE = ResourceType.Magmavite;
        
        public int WarHealth => _warHealth; 
        public int PlayerSimulatedHealth => _enemySimulatedHealth;
        
        public event Action<string> OnBattleLog;

        public BaseBehaviour(Entity.Civilization.Civilization civ, CommandInvoker invoker)
        {
            _civilization = civ;
            _invoker = invoker;
            InitializeUtilitySystem();
            InitializeBehaviourTree();
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
                if (x <= 0.6f) return 0f;
                float X = x * 100f;
                double numerator = Math.Exp(0.1 * (X - 60)) - 1;
                double denominator = Math.Exp(4) - 1;
                return (float)(numerator / denominator);
            };
            _curveFactors["DependencyCurve"] = dependencyCurve;
            
            CustomCurveFactor independencyCurve = UtilitySystem.CreateCurve<CustomCurveFactor>(vDependability);
            independencyCurve.Function = (x) =>
            {
                if (x <= 0.6f) return 1f;
                float X = x * 100f;
                double numerator = Math.Exp(-0.1 * (X - 60)) - Math.Exp(-4);
                double denominator = 1 - Math.Exp(-4);
                return (float)(numerator / denominator);
            };
            _curveFactors["IndependencyCurve"] = independencyCurve;
            
            PointedCurveFactor interestCurve = UtilitySystem.CreateCurve<PointedCurveFactor>(vInterest);
            interestCurve.Points = new List<CurvePoint>
            {
                new CurvePoint(0f, 0f), new CurvePoint(0.5f, 0f), new CurvePoint(0.8f, 1f), new CurvePoint(1f, 1f)
            };
            _curveFactors["InterestCurve"] = interestCurve;
            
            PointedCurveFactor indifirentCurve = UtilitySystem.CreateCurve<PointedCurveFactor>(vInterest);
            indifirentCurve.Points = new List<CurvePoint>
            {
                new CurvePoint(0f, 1f), new CurvePoint(0.5f, 1f), new CurvePoint(0.8f, 0f), new CurvePoint(1f, 0f)
            };
            _curveFactors["IndifirentCurve"] = indifirentCurve;
            
            CustomCurveFactor distrustCurve = UtilitySystem.CreateCurve<CustomCurveFactor>(vTrustworthiness);
            distrustCurve.Function = (x) =>
            {
                if (x <= 0.8f) return (float)(1 - Math.Pow((x/0.8f), 2));
                return (float)(6.25f*Math.Pow((x - 0.8f), 2));
            };
            _curveFactors["DistrustCurve"] = distrustCurve;
            
            CustomCurveFactor faithCurve = UtilitySystem.CreateCurve<CustomCurveFactor>(vTrustworthiness);
            faithCurve.Function = (x) =>
            {
                if (x <= 0.8f) return (float)Math.Pow((x/0.8f), 2);
                return (float)(1 - 18.75f*Math.Pow((x - 0.8f), 2));
            };
            _curveFactors["FaithCurve"] = faithCurve;
            
            // ACCIONES DEL UTILITY SYSTEM
            _actions["SetDisgusted"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Disgusted); return Status.Success; }, () => { });
            _actions["SetProgressive"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Progressive); return Status.Success; }, () => { });
            _actions["SetNeeded"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Needed); return Status.Success; }, () => { });
            _actions["SetAlly"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Ally); return Status.Success; }, () => { });
            _actions["SetCommerce"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Commerce); return Status.Success; }, () => { });
            _actions["SetLove"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Love); return Status.Success; }, () => { });
            _actions["SetNegotiation"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Negotiation); return Status.Success; }, () => { });
            
            FunctionalAction setProgressive = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Progressive); return Status.Success; }, () => { });
            _actions["SetProgressive"] = setProgressive;
            
            FunctionalAction setNeeded = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Needed); return Status.Success; }, () => { });
            _actions["SetNeeded"] = setNeeded;
            
            FunctionalAction setAlly = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Ally); return Status.Success; }, () => { });
            _actions["SetAlly"] = setAlly;
            
            FunctionalAction setCommerce = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Commerce); return Status.Success; }, () => { });
            _actions["SetCommerce"] = setCommerce;
            
            FunctionalAction setLove = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Love); return Status.Success; }, () => { });
            _actions["SetLove"] = setLove;
            
            FunctionalAction setNegotiation = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Negotiation); return Status.Success; }, () => { });
            _actions["SetNegotiation"] = setNegotiation;
            
            FunctionalAction setBelligerent = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Belligerent); return Status.Success; }, () => { });
            _actions["SetBelligerent"] = setBelligerent;
            
            FunctionalAction setPeaceful = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Peaceful); return Status.Success; }, () => { });
            _actions["SetPeaceful"] = setPeaceful;
            
            FunctionalAction demandTribute = new FunctionalAction(
                () => { },
                () =>
                {
                    _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Offended); 
                    QuickTradeManager.Instance.CreateTributeOffer(_civilization);
                    return Status.Success;
                }, 
                () => { });
            _actions["DemandTribute"] = demandTribute;
            FunctionalAction offerGift = new FunctionalAction(
                () => { },
                () =>
                {
                    _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Generous);
                    QuickTradeManager.Instance.CreateGiftOffer(_civilization);
                    return Status.Success;
                }, 
                () => { });
            _actions["OfferGift"] = offerGift;
            
            FunctionalAction setOffended = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Offended); return Status.Success; }, () => { });
            _actions["SetOffended"] = setOffended;
            
            FunctionalAction declareWar = new FunctionalAction(
                
                () => Debug.Log("Declare War"),
                () =>
                {
                    _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Belligerent); 
                    SystemEvents.OnWarDeclaredToPlayer?.Invoke(_civilization);
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
            
            FunctionalAction seekHelp = new FunctionalAction(
                
                () => Debug.Log("Seek Help"),
                () =>
                {
                    _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Needed); 
                    QuickTradeManager.Instance.CreateQuestOffer(_civilization);
                    return Status.Success;
                },
                () => Debug.Log("Sought Help")
            );
            _actions["SeekHelp"] = seekHelp;
            
            FunctionalAction increaseTrade = new FunctionalAction(
                
                () => Debug.Log("Increase Trade"),
                () =>
                {
                    Debug.Log("Increase Trade");
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
                    _civilization.CivilizationState.FriendlinessLevel = 1f;
                    _civilization.CivilizationState.InterestLevel = 1f;
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

            _actions["SetBelligerent"] = new FunctionalAction(
                () => { }, 
                () => { 
                    _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Belligerent);
                    TryTriggerWarDeclaration(); 
                    return Status.Success; 
                }, 
                () => { }
            );
            _actions["SetBelligerent"] = _actions["SetBelligerent"];
        }

        private void LogBattle(string message)
        {
            Debug.Log(message);
            OnBattleLog?.Invoke(message);
        }
        
        public void TakeDamageFromPlayer(int damage)
        {
            _warHealth -= damage;
            SystemEvents.TriggerWarHealthUpdated(_warHealth, _enemySimulatedHealth);
    
            LogBattle($"<color=yellow>{_civilization.CivilizationData.Name} took direct hit!</color>");
        }
        
        private FunctionalAction CreateWaitAction()
        {
            return new FunctionalAction(
                () => { _waitTimer = 0f; },
                () => {
                    _waitTimer += UnityEngine.Time.deltaTime;
                    if (_waitTimer >= 1.5f) return Status.Success; 
                    return Status.Running; 
                }, 
                () => {}
            );
        }

        protected virtual void InitializeBehaviourTree()
        {
            string civName = _civilization.CivilizationData.Name;
        
            Debug.Log($"<color=green>[BT SETUP] Initializing War Tree for {civName}...</color>");
            _warTree = new BehaviourTree();
        
            // RAMA 1: SURVIVAL
            
            var actCheckHealth = new FunctionalAction(() => {}, () => {
                bool dying = _warHealth <= 0;
                if(dying) LogBattle($"<color=orange>Critical systems failure.  Surrendering.</color>");
                return dying ? Status.Success : Status.Failure; 
            }, () => {});
        
            var actSurrender = new FunctionalAction(() => {}, () => { 
                LogBattle($"<color=red>SURRENDER signal sent.</color>"); 
                StopWar(); 
                return Status.Success; 
            }, () => {});
        
            var nCheckHealth = _warTree.CreateLeafNode("CheckHealth", actCheckHealth);
            var nSurrender = _warTree.CreateLeafNode("Surrender", actSurrender);
            
            var seqSurvival = _warTree.CreateComposite<SequencerNode>("SurvivalSequence", false, nCheckHealth, nSurrender);
        
            // RAMA 2: ATTACK
        
            var actCheckAmmo = new FunctionalAction(() => {}, () => {
                bool hasAmmo = CheckInventoryForFireStrike();
                return hasAmmo ? Status.Success : Status.Failure; 
            }, () => {});
        
            var actFire = new FunctionalAction(() => {}, () => { 
                LogBattle($"<color=red>Firing missile!</color>"); 
                ConsumeFireStrike(); 
                return Status.Success; 
            }, () => {});
        
            var actCheckHit = new FunctionalAction(() => {}, () => {
                float hitChance = 0.25f;
                bool hit = UnityEngine.Random.value <= hitChance;
                if(hit) LogBattle($"<color=orange>Impact confirmed!</color>");
                else LogBattle($"<color=orange>Fire Strike Missed Target.</color>");
                return hit ? Status.Success : Status.Failure; 
            }, () => {});
        
            var actDamage = new FunctionalAction(() => {}, () => { 
                DamagePlayerPlanet();
                return Status.Success; 
            }, () => {});
        
            var actCheckEnemyDead = new FunctionalAction(() => {}, () => {
                return (_enemySimulatedHealth <= 0) ? Status.Success : Status.Failure;
            }, () => {});
        
            var actWin = new FunctionalAction(() => {}, () => {
                LogBattle($"<color=red>[{civName}] VICTORY.</color>");
                StopWar();
                return Status.Success;
            }, () => {});
        
            var actContinueWar = new FunctionalAction(() => {}, () => { return Status.Success; }, () => {});
        
            var nCheckAmmo = _warTree.CreateLeafNode("CheckAmmo", actCheckAmmo);
            var nFire = _warTree.CreateLeafNode("Fire", actFire);
            var nCheckHit = _warTree.CreateLeafNode("CheckHit", actCheckHit);
            var nDamage = _warTree.CreateLeafNode("Damage", actDamage);
            var nCheckEnemyDead = _warTree.CreateLeafNode("CheckEnemyDead", actCheckEnemyDead);
            var nWin = _warTree.CreateLeafNode("WinActions", actWin);
            var nContinue = _warTree.CreateLeafNode("Continue", actContinueWar);
        
            var seqVictory = _warTree.CreateComposite<SequencerNode>("VictorySeq", false, nCheckEnemyDead, nWin);
            var selVictoryOutcome = _warTree.CreateComposite<SelectorNode>("VictorySelector", false, seqVictory, nContinue);
            
            var seqAttack = _warTree.CreateComposite<SequencerNode>("AttackSequence", false, 
                nCheckAmmo, nFire, nCheckHit, nDamage, selVictoryOutcome);
        
            // RAMA 3: LOGISTICS
        
            var actCheckResources = new FunctionalAction(() => {}, () => {
                bool hasRes = _civilization.StorageSystem.HasResource(ResourceType.Magmavite, COST_FIRE_STRIKE);
                return hasRes ? Status.Success : Status.Failure;
            }, () => {});
        
            var actCraftAmmo = new FunctionalAction(() => {}, () => {
                LogBattle($"<color=red>Crafting Fire Strike...</color>");
                _civilization.StorageSystem.ConsumeResource(ResourceType.Magmavite, COST_FIRE_STRIKE);
                _civilization.StorageSystem.AddInventoryItem(ITEM_FIRE_STRIKE, 1);
                return Status.Success;
            }, () => {});
        
            var actGather = new FunctionalAction(() => {}, () => {
                _civilization.StorageSystem.AddResource(TYPE_MAGMAVITE, GATHER_AMOUNT);
                int current = _civilization.StorageSystem.GetResourceAmount(TYPE_MAGMAVITE);
                LogBattle($"<color=red>Fire Strike Charge [{current}/{COST_FIRE_STRIKE}]</color>");
                
                return Status.Success; 
            }, () => {});
        
            var nCheckRes = _warTree.CreateLeafNode("CheckRes", actCheckResources);
            var nCraft = _warTree.CreateLeafNode("Craft", actCraftAmmo);
            var nGather = _warTree.CreateLeafNode("Gather", actGather);
        
            var seqCrafting = _warTree.CreateComposite<SequencerNode>("TryCraftSequence", false, nCheckRes, nCraft);
            var seqGathering = _warTree.CreateComposite<SequencerNode>("GatherSeq", false, nGather);
            
            var selLogistics = _warTree.CreateComposite<SelectorNode>("LogisticsSelector", false, seqCrafting, seqGathering);
        
            // RAÍZ
        
            var rootSelector = _warTree.CreateComposite<SelectorNode>("MainSelector", false, seqSurvival, seqAttack, selLogistics);
        
            var rootLoop = _warTree.CreateDecorator<LoopNode>("RootLoop", rootSelector);
            rootLoop.SetIterations(-1);
        
            _warTree.SetRootNode(rootLoop);
            _rootNode = rootLoop; 
        }
        
        public void DEBUG_ForceWarSituation()
        {
            SystemEvents.TriggerWarDeclared(_civilization);
        }
        
        private void TryTriggerWarDeclaration()
        {
            if (_isAtWarWithPlayer) return;
            SystemEvents.TriggerWarDeclared(_civilization);
        }

        public void StartWar()
        {
            _isAtWarWithPlayer = true;
            _warTree.Start();
            _warHealth = 100;
            _enemySimulatedHealth = 100;
        }

        public void StopWar()
        {
            _isAtWarWithPlayer = false;
            SystemEvents.TriggerPeaceSigned(_civilization);
        }

        public void TakeDamage(int amount)
        {
            _warHealth -= amount;
        }

        private bool CheckInventoryForFireStrike()
        {
            return _civilization.StorageSystem.GetItemCount(ITEM_FIRE_STRIKE) > 0;
        }

        private void ConsumeFireStrike()
        {
            _civilization.StorageSystem.ConsumeInventoryItem(ITEM_FIRE_STRIKE, 1);
        }

        private void DamagePlayerPlanet()
        {
            _enemySimulatedHealth -= 25;
            SystemEvents.TriggerWarHealthUpdated(_warHealth, _enemySimulatedHealth);

            Debug.Log($"[WAR] Player damaged. Remaining Health: {_enemySimulatedHealth}");
        }

        public void UpdateAI(WorldContext context)
        {
            if (_isAtWarWithPlayer)
            {
                _warTree.Update();
            }
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