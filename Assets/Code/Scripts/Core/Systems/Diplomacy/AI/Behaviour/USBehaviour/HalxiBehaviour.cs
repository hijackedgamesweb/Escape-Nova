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
            
            //Beligerantes
            WeightedFusionFactor enemyFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["EnemyCurve"], _curveFactors["DistrustCurve"]);
            enemyFusion.Weights = new float[]{ 0.8f, 0.2f };
            
            //Necesitados
            WeightedFusionFactor neededFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["DependencyCurve"], _curveFactors["FaithCurve"]);
            neededFusion.Weights = new float[]{ 0.6f, 0.4f };
            
            //Generosos
            WeightedFusionFactor loveFusion = UtilitySystem.CreateFusion<WeightedFusionFactor>(_curveFactors["FriendshipCurve"], _curveFactors["IndependencyCurve"]);
            loveFusion.Weights = new float[]{ 0.3f, 0.7f };
            
            
            //Generosos
            UtilitySystem.CreateAction(loveFusion, _actions["OfferGift"]);
            
            //Beligerantes
            UtilitySystem.CreateAction(enemyFusion, _actions["DeclareWar"]);
            
            //Necesitados
            UtilitySystem.CreateAction(neededFusion, _actions["SeekHelp"]);
        }
    }
}