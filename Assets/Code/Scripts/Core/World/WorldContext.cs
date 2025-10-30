namespace Code.Scripts.Core.World
{
    public class WorldContext
    {
        public float CurrentTurn { get; private set; }
        public Entity.Player.Player Player { get; set; }
        
        public WorldContext(float currentTurn, Entity.Player.Player player)
        {
            Player = player;
            CurrentTurn = currentTurn;
        }
    }
}