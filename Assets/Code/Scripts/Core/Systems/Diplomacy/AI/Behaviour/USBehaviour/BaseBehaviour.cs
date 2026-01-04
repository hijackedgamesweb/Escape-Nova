using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UtilitySystems;
using BehaviourAPI.BehaviourTrees;
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
        
        // --- VARIABLES DE GUERRA ---
        private BehaviourTree _warTree;
        private Node _rootNode; // Referencia para el Loop
        private int _warHealth = 100;
        private bool _isAtWarWithPlayer = false;
        private float _waitTimer = 0f;
        
        // Simulación de vida del enemigo (Jugador) para la lógica de victoria de la IA
        private int _enemySimulatedHealth = 100; 

        private const int COST_FIRE_STRIKE = 500;   
        private const int GATHER_AMOUNT = 10;
        private const string ITEM_FIRE_STRIKE = "Fire Strike";
        private const string RES_MAGMAVITE = "Magmavite";

        // --- EVENTOS ---
        public static event Action<Entity.Civilization.Civilization> OnWarDeclaredToPlayer; 
        public static event Action<Entity.Civilization.Civilization> OnPeaceSigned;
        public static event Action<int, int> OnWarHealthUpdated;
        public event Action<string> OnBattleLog;
        // ---------------------------------------

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
            
            _actions["SetBelligerent"] = new FunctionalAction(
                () => { }, 
                () => { 
                    _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Belligerent);
                    TryTriggerWarDeclaration(); 
                    return Status.Success; 
                }, 
                () => { }
            );
            _actions["SetBelligerent"] = _actions["SetBelligerent"]; // (Redundante pero mantiene estructura)
            
            _actions["SetPeaceful"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Peaceful); return Status.Success; }, () => { });
            _actions["SetGenerous"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Generous); return Status.Success; }, () => { });
            _actions["SetOffended"] = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Offended); return Status.Success; }, () => { });
            
            _actions["OfferPeace"] = new FunctionalAction(() => Debug.Log("Offer Peace"), () => { Debug.Log("Offering Peace"); return Status.Success; }, () => Debug.Log("Offered Peace"));
            _actions["DeclareWar"] = new FunctionalAction(() => Debug.Log("Declare War"), () => { Debug.Log("Declaring War"); return Status.Success; }, () => Debug.Log("Declared War"));
            _actions["ProposeAlliance"] = new FunctionalAction(() => Debug.Log("Propose Alliance"), () => { Debug.Log("Proposing Alliance"); return Status.Success; }, () => Debug.Log("Proposed Alliance"));
            _actions["ProposeMarriage"] = new FunctionalAction(() => Debug.Log("Propose Marriage"), () => { Debug.Log("Proposing Marriage"); return Status.Success; }, () => Debug.Log("Proposed Marriage"));
            _actions["SeekHelp"] = new FunctionalAction(() => Debug.Log("Seek Help"), () => { Debug.Log("Seeking Help"); return Status.Success; }, () => Debug.Log("Sought Help"));
            _actions["IncreaseTrade"] = new FunctionalAction(() => Debug.Log("Increase Trade"), () => { Debug.Log("Increase Trade"); return Status.Success; }, () => Debug.Log("Increased Trade"));
            _actions["Runaway"] = new FunctionalAction(() => Debug.Log("Run Away"), () => { Debug.Log("Running Away"); _civilization.CivilizationState.FriendlinessLevel = 1f; _civilization.CivilizationState.InterestLevel = 1f; return Status.Success; }, () => Debug.Log("Ran Away"));
            _actions["ProposeInvestigation"] = new FunctionalAction(() => Debug.Log("Propose Investigation"), () => { Debug.Log("Proposing Investigation"); return Status.Success; }, () => Debug.Log("Proposed Investigation"));
            _actions["ExchangeTreaty"] = new FunctionalAction(() => Debug.Log("Exchange Treaty"), () => { Debug.Log("Exchanging Treaty"); return Status.Success; }, () => Debug.Log("Exchanged Treaty"));
            _actions["GiveAid"] = new FunctionalAction(() => Debug.Log("Give Aid"), () => { Debug.Log("Giving Aid"); return Status.Success; }, () => Debug.Log("Gave Aid"));
        }

        private void LogBattle(string message)
        {
            Debug.Log(message);
            OnBattleLog?.Invoke(message);
        }
        
        public void TakeDamageFromPlayer(int damage)
        {
            _warHealth -= damage;
            // Notificamos a la UI
            OnWarHealthUpdated?.Invoke(_warHealth, _enemySimulatedHealth);
    
            // Log actualizado a inglés y formato pedido
            LogBattle($"<color=green>[SYSTEM] {_civilization.CivilizationData.Name} took direct hit! Remaining Integrity: {_warHealth}%</color>");
        }
        
        private FunctionalAction CreateWaitAction()
        {
            return new FunctionalAction(
                () => { _waitTimer = 0f; }, // OnStart
                () => { 
                    // SOLUCIÓN ERROR 1: Usamos UnityEngine.Time explícitamente
                    _waitTimer += UnityEngine.Time.deltaTime;
                    if (_waitTimer >= 1.5f) return Status.Success; 
                    return Status.Running; 
                }, 
                () => {} // OnStop
            );
        }

        protected virtual void InitializeBehaviourTree()
        {
            string civName = _civilization.CivilizationData.Name;
        
            Debug.Log($"<color=green>[BT SETUP] Initializing War Tree for {civName}...</color>");
            _warTree = new BehaviourTree();
        
            // ==========================================================================================
            // RAMA 1: SURVIVAL
            // ==========================================================================================
            
            var actCheckHealth = new FunctionalAction(() => {}, () => {
                bool dying = _warHealth <= 0;
                if(dying) LogBattle($"<color=green>[{civName}] Critical systems failure. Surrendering.</color>");
                return dying ? Status.Success : Status.Failure; 
            }, () => {});
        
            var actSurrender = new FunctionalAction(() => {}, () => { 
                LogBattle($"<color=green>[{civName}] SURRENDER signal sent.</color>"); 
                StopWar(); 
                return Status.Success; 
            }, () => {});
        
            var nCheckHealth = _warTree.CreateLeafNode("CheckHealth", actCheckHealth);
            var nSurrender = _warTree.CreateLeafNode("Surrender", actSurrender);
            
            // SOLUCIÓN ERROR 2: Creamos un nodo de espera ÚNICO para esta rama
            var nWaitSurvival = _warTree.CreateLeafNode("WaitSurvival", CreateWaitAction());
        
            var seqSurvival = _warTree.CreateComposite<SequencerNode>("SurvivalSequence", false, nCheckHealth, nSurrender, nWaitSurvival);
        
            // ==========================================================================================
            // RAMA 2: ATTACK
            // ==========================================================================================
        
            var actCheckAmmo = new FunctionalAction(() => {}, () => {
                bool hasAmmo = CheckInventoryForFireStrike();
                return hasAmmo ? Status.Success : Status.Failure; 
            }, () => {});
        
            var actFire = new FunctionalAction(() => {}, () => { 
                LogBattle($"<color=red>[{civName}] Firing missile!</color>"); 
                ConsumeFireStrike(); 
                return Status.Success; 
            }, () => {});
        
            var actCheckHit = new FunctionalAction(() => {}, () => {
                float hitChance = 0.6f; 
                bool hit = UnityEngine.Random.value <= hitChance;
                if(hit) LogBattle($"<color=red>[{civName}] Impact confirmed!</color>");
                else LogBattle($"<color=white>[{civName}] Missed target.</color>");
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
                LogBattle($"<color=yellow>[{civName}] VICTORY.</color>");
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
            
            // SOLUCIÓN ERROR 2: Otro nodo de espera ÚNICO para esta rama
            var nWaitAttack = _warTree.CreateLeafNode("WaitAttack", CreateWaitAction());
        
            var seqAttack = _warTree.CreateComposite<SequencerNode>("AttackSequence", false, 
                nCheckAmmo, nFire, nCheckHit, nDamage, selVictoryOutcome, nWaitAttack);
        
            // ==========================================================================================
            // RAMA 3: LOGISTICS
            // ==========================================================================================
        
            var actCheckResources = new FunctionalAction(() => {}, () => {
                bool hasRes = _civilization.StorageSystem.HasResource(ResourceType.Magmavite, COST_FIRE_STRIKE);
                if(!hasRes) LogBattle($"<color=orange>[{civName}] Low resources. Gathering...</color>");
                return hasRes ? Status.Success : Status.Failure;
            }, () => {});
        
            var actCraftAmmo = new FunctionalAction(() => {}, () => {
                LogBattle($"<color=blue>[{civName}] Crafting ammo...</color>");
                _civilization.StorageSystem.ConsumeResource(ResourceType.Magmavite, COST_FIRE_STRIKE);
                _civilization.StorageSystem.AddInventoryItem(ITEM_FIRE_STRIKE, 1);
                return Status.Success;
            }, () => {});
        
            var actGather = new FunctionalAction(() => {}, () => {
                LogBattle($"<color=blue>[{civName}] Gathering Magmavite...</color>");
                _civilization.StorageSystem.AddResource(ResourceType.Magmavite, GATHER_AMOUNT);
                return Status.Success; 
            }, () => {});
        
            var nCheckRes = _warTree.CreateLeafNode("CheckRes", actCheckResources);
            var nCraft = _warTree.CreateLeafNode("Craft", actCraftAmmo);
            var nGather = _warTree.CreateLeafNode("Gather", actGather);
        
            // SOLUCIÓN ERROR 2: Nodos de espera únicos
            var nWaitCraft = _warTree.CreateLeafNode("WaitCraft", CreateWaitAction());
            var nWaitGather = _warTree.CreateLeafNode("WaitGather", CreateWaitAction());

            var seqCrafting = _warTree.CreateComposite<SequencerNode>("TryCraftSequence", false, nCheckRes, nCraft, nWaitCraft);
            var seqGathering = _warTree.CreateComposite<SequencerNode>("GatherSeq", false, nGather, nWaitGather);
            
            var selLogistics = _warTree.CreateComposite<SelectorNode>("LogisticsSelector", false, seqCrafting, seqGathering);
        
            // ==========================================================================================
            // RAÍZ
            // ==========================================================================================
        
            var rootSelector = _warTree.CreateComposite<SelectorNode>("MainSelector", false, seqSurvival, seqAttack, selLogistics);
        
            var rootLoop = _warTree.CreateDecorator<LoopNode>("RootLoop", rootSelector);
            rootLoop.SetIterations(-1);
        
            _warTree.SetRootNode(rootLoop);
            _rootNode = rootLoop; 
        }
        
        public void DEBUG_ForceWarSituation()
        {
            Debug.Log($"[DEBUG] Forzando situación de guerra para {_civilization.CivilizationData.Name}...");
            _civilization.StorageSystem.AddInventoryItem(ITEM_FIRE_STRIKE, 2);
            _civilization.StorageSystem.AddResource(ResourceType.Magmavite, 500);
            OnWarDeclaredToPlayer?.Invoke(_civilization);
        }
        
        private void TryTriggerWarDeclaration()
        {
            if (_isAtWarWithPlayer) return;

            if (CheckInventoryForFireStrike())
            {
                Debug.Log($"[US-Personality] {_civilization.CivilizationData.Name} decide declarar la guerra.");
                OnWarDeclaredToPlayer?.Invoke(_civilization);
            }
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
            OnPeaceSigned?.Invoke(_civilization);
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
            OnWarHealthUpdated?.Invoke(_warHealth, _enemySimulatedHealth);

            Debug.Log($"[WAR] Player damaged. Remaining Health: {_enemySimulatedHealth}");
        }

        public void UpdateAI(WorldContext context)
        {
            if (_isAtWarWithPlayer)
            {
                // Solo logueamos de vez en cuando para no saturar consola, o lo quitamos
                // Debug.Log(">>> [UPDATE] Tick..."); 
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