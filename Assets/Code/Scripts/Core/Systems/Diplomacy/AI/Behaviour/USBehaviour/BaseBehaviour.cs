using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UtilitySystems;
using BehaviourAPI.BehaviourTrees;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Action = System.Action;

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
        
        // --- VARIABLES NUEVAS PARA LA GUERRA ---
        private BehaviourTree _warTree;
        private int _warHealth = 100;
        private bool _isAtWarWithPlayer = false;
        
        private const int COST_FIRE_STRIKE = 50;   // Cuánto cuesta hacer una bala
        private const int GATHER_AMOUNT = 10;

        public static event Action<Entity.Civilization.Civilization> OnWarDeclaredToPlayer; 
        public static event Action<Entity.Civilization.Civilization> OnPeaceSigned;
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
                if (x <= 0.6f)
                {
                    return 0f;
                }
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
                {
                    return 1f;
                }
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
                {
                    return (float)(1 - Math.Pow((x/0.8f), 2));
                }
                return (float)(6.25f*Math.Pow((x - 0.8f), 2));
            };
            _curveFactors["DistrustCurve"] = distrustCurve;
            
            CustomCurveFactor faithCurve = UtilitySystem.CreateCurve<CustomCurveFactor>(vTrustworthiness);
            faithCurve.Function = (x) =>
            {
                if (x <= 0.8f)
                {
                    return (float)Math.Pow((x/0.8f), 2);
                }
                return (float)(1 - 18.75f*Math.Pow((x - 0.8f), 2));
            };
            _curveFactors["FaithCurve"] = faithCurve;
            
            FunctionalAction setDisgusted = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Disgusted); return Status.Success; }, () => { });
            _actions["SetDisgusted"] = setDisgusted;
            
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
            
            // MODIFICADO: Añadido el trigger de guerra aquí
            FunctionalAction setBelligerent = new FunctionalAction(
                () => { }, 
                () => { 
                    _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Belligerent);
                    TryTriggerWarDeclaration(); // <--- GATILLO DE GUERRA AÑADIDO
                    return Status.Success; 
                }, 
                () => { }
            );
            _actions["SetBelligerent"] = setBelligerent;
            
            FunctionalAction setPeaceful = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Peaceful); return Status.Success; }, () => { });
            _actions["SetPeaceful"] = setPeaceful;
            
            FunctionalAction setGenerous = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Generous); return Status.Success; }, () => { });
            _actions["SetGenerous"] = setGenerous;
            
            FunctionalAction setOffended = new FunctionalAction(() => { }, () => { _civilization.CivilizationState.SetCurrentMood(Entity.EntityMood.Offended); return Status.Success; }, () => { });
            _actions["SetOffended"] = setOffended;
            
            
            
            
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
        }

        // MÉTODO DE INICIALIZACIÓN DEL ÁRBOL DE COMPORTAMIENTO
        private void InitializeBehaviourTree()
        {
            _warTree = new BehaviourTree();

            // --- RAMA 1: SUPERVIVENCIA
            var actCheckHealth = new FunctionalAction(() => {}, () => (_warHealth <= 0) ? Status.Success : Status.Failure, () => {});
            var actSurrender = new FunctionalAction(() => {}, () => { Debug.Log($"[BT] {_civilization.CivilizationData.Name} se rinde."); StopWar(); return Status.Success; }, () => {});

            // --- RAMA 2: ATAQUE
            var actCheckAmmo = new FunctionalAction(() => {}, () => CheckInventoryForFireStrike() ? Status.Success : Status.Failure, () => {});
            var actFire = new FunctionalAction(() => {}, () => { Debug.Log($"[BT] {_civilization.CivilizationData.Name} dispara."); ConsumeFireStrike(); return Status.Success; }, () => {});
            var actCheckHit = new FunctionalAction(() => {}, () => (UnityEngine.Random.value > 0.3f) ? Status.Success : Status.Failure, () => {}); // 70% acierto
            var actDamage = new FunctionalAction(() => {}, () => { DamagePlayerPlanet(); return Status.Success; }, () => {});

            // --- RAMA 3: LOGÍSTICA (Crafteo y Recolección)
            var actCheckResources = new FunctionalAction(() => {}, () => {
                return _civilization.StorageSystem.HasResource(ResourceType.Magmavite, COST_FIRE_STRIKE) ? Status.Success : Status.Failure;
            }, () => {});

            // Acción: Crear munición (Consumir recursos -> Dar bala)
            var actCraftAmmo = new FunctionalAction(() => {}, () => {
                Debug.Log($"[BT] Fabricando 'Fire Strike'...");
                _civilization.StorageSystem.ConsumeResource(ResourceType.Magmavite, COST_FIRE_STRIKE);
                _civilization.StorageSystem.AddInventoryItem("Fire Strike", 1);
                return Status.Success;
            }, () => {});

            // Acción: Recolectar (Nunca falla, siempre produce)
            var actGather = new FunctionalAction(() => {}, () => {
                Debug.Log($"[BT] Recolectando recursos...");
                _civilization.StorageSystem.AddResource(ResourceType.Magmavite, GATHER_AMOUNT);
                return Status.Failure;
            }, () => {});


            //
            // CONSTRUCCIÓN DEL ÁRBOL
            //

            // Nodos Hoja
            var nCheckHealth = _warTree.CreateLeafNode("CheckHealth", actCheckHealth);
            var nSurrender = _warTree.CreateLeafNode("Surrender", actSurrender);
            
            var nCheckAmmo = _warTree.CreateLeafNode("CheckAmmo", actCheckAmmo);
            var nFire = _warTree.CreateLeafNode("Fire", actFire);
            var nCheckHit = _warTree.CreateLeafNode("CheckHit", actCheckHit);
            var nDamage = _warTree.CreateLeafNode("Damage", actDamage);

            // Nodos Logística
            var nCheckRes = _warTree.CreateLeafNode("CheckRes", actCheckResources);
            var nCraft = _warTree.CreateLeafNode("CraftAmmo", actCraftAmmo);
            var nGather = _warTree.CreateLeafNode("Gather", actGather);
            
            // Secuencias (Estructura Vertical)
            var seqPreservation = _warTree.CreateComposite<SequencerNode>("SelfPreservation", false, nCheckHealth, nSurrender);
            var seqAttack = _warTree.CreateComposite<SequencerNode>("Attack", false, nCheckAmmo, nFire, nCheckHit, nDamage);

            // LOGÍSTICA: Es una Secuencia (Tengo materiales -> Fabrico)
            var seqCrafting = _warTree.CreateComposite<SequencerNode>("Crafting", false, nCheckRes, nCraft);
            var selLogistics = _warTree.CreateComposite<SelectorNode>("LogisticsSelector", false, seqCrafting, nGather);
            var rootSelector = _warTree.CreateComposite<SelectorNode>("RootSelector", false, seqPreservation, seqAttack, selLogistics);

            _warTree.SetRootNode(rootSelector);
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
            _warHealth = 100;
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
            return _civilization.StorageSystem.GetItemCount("Fire Strike") > 0;
        }

        private void ConsumeFireStrike()
        {
            _civilization.StorageSystem.ConsumeInventoryItem("Fire Strike", 1);
        }

        private void DamagePlayerPlanet()
        {
            Debug.Log($"[WAR] {_civilization.CivilizationData.Name} ha dañado un planeta del jugador.");
        }

        public void UpdateAI(WorldContext context)
        {
            throw new NotImplementedException();
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