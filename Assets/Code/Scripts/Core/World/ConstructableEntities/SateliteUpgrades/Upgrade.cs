namespace Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades
{
    [System.Serializable]
    public abstract class Upgrade
    {
        public abstract void ApplyUpgrade(Planet planet);
    }
}