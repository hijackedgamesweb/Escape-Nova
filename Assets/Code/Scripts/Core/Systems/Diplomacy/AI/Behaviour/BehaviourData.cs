namespace Code.Scripts.Core.Systems.Civilization.AI.Behaviour
{
    public class BehaviourData
    {
        public float Friendliness { get; set; }
        public float Dependability { get; set; }
        public float Interest { get; set; }
        public float Trustworthiness { get; set; }
        
        public BehaviourData(float friendliness, float dependability, float interest, float trustworthiness)
        {
            Friendliness = friendliness;
            Dependability = dependability;
            Interest = interest;
            Trustworthiness = trustworthiness;
        }
    }
}