using BehaviourAPI.UtilitySystems;
using Code.Scripts.Patterns.Command;

namespace Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour
{
    public class HalxiBehaviour : BaseBehaviour
    {
        public HalxiBehaviour(Entity.Civilization.Civilization civ, CommandInvoker invoker) : base(civ, invoker)
        {
        }
        
        protected override void InitializeUtilitySystem()
        {
            base.InitializeUtilitySystem();
            
            WeightedFusionFactor peaceFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            peaceFusion.Weights = new float[]{ 0.2f, 0.1f, 0.7f };
            
            WeightedFusionFactor allianceFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["FaithCurve"]);
            allianceFusion.Weights = new float[]{ 0.7f, 0.3f };
            
            WeightedFusionFactor disgustedFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["IndependencyCurve"], _curveFactors["IndifirentCurve"], _curveFactors["DistrustCurve"]);
            disgustedFusion.Weights = new float[]{ 0.6f, 0.1f, 0.15f, 0.15f };
            
            WeightedFusionFactor neededFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["DependencyCurve"], _curveFactors["FaithCurve"]);
            neededFusion.Weights = new float[]{ 0.7f, 0.15f, 0.15f };
            
            WeightedFusionFactor progressiveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["InterestCurve"]);
            progressiveFusion.Weights = new float[]{ 0.8f, 0.2f };
            
            WeightedFusionFactor commerceFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["DependencyCurve"], _curveFactors["InterestCurve"]);
            commerceFusion.Weights = new float[]{ 0.4f, 0.6f };
            
            WeightedFusionFactor belligerentFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["InterestCurve"], _curveFactors["DistrustCurve"]);
            belligerentFusion.Weights = new float[]{ 0.8f, 0.1f, 0.1f };
            
            WeightedFusionFactor negotiationFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            negotiationFusion.Weights = new float[]{ 0.3f, 0.7f };
            
            WeightedFusionFactor generosityFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["InterestCurve"], _curveFactors["FaithCurve"]);
            generosityFusion.Weights = new float[]{ 0.7f, 0.15f, 0.15f };
            
            UtilityAction runawayAction = UtilitySystem.CreateAction(disgustedFusion, _actions["Runaway"]);
            UtilityAction offerPeaceAction = UtilitySystem.CreateAction(peaceFusion, _actions["OfferPeace"]);
            UtilityAction declareWarAction = UtilitySystem.CreateAction(belligerentFusion, _actions["DeclareWar"]);
            UtilityAction proposeAllianceAction = UtilitySystem.CreateAction(allianceFusion, _actions["ProposeAlliance"]);
            UtilityAction seekHelpAction = UtilitySystem.CreateAction(neededFusion, _actions["SeekHelp"]);
            UtilityAction increaseTradeAction = UtilitySystem.CreateAction(commerceFusion, _actions["IncreaseTrade"]);
            UtilityAction proposeInvestigationAction = UtilitySystem.CreateAction(progressiveFusion, _actions["ProposeInvestigation"]);
            UtilityAction exchangeTreatyAction = UtilitySystem.CreateAction(negotiationFusion, _actions["ExchangeTreaty"]);
            UtilityAction giveAidAction = UtilitySystem.CreateAction(generosityFusion, _actions["GiveAid"]);

            
        }
    }
}