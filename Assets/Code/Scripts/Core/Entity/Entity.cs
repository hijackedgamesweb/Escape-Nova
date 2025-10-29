using Code.Scripts.Patterns.Command;
using Code.Scripts.Player;

namespace Code.Scripts.Core.Entity
{
    public class Entity
    {
        protected CommandInvoker _invoker;
        public EntityData EntityData { get; private set; }
        public EntityState EntityState{ get; private set; }
        
        public Entity(CommandInvoker invoker, EntitySO entitySo)
        {
            _invoker = invoker;
            EntityData = new EntityData(entitySo);
            EntityState = new EntityState(entitySo);
        }
        
    }
}