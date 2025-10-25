namespace Code.Scripts.Core.World
{
    public class WorldContext
    {
        public float CurrentTurn { get; private set; }
        
        public WorldContext(float currentTurn)
        {
            CurrentTurn = currentTurn;
        }
    }
}