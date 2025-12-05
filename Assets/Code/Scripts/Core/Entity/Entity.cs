using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Player;

namespace Code.Scripts.Core.Entity
{
    public class Entity
    {
        protected CommandInvoker _invoker;
        public EntityData EntityData { get; private set; }
        public EntityState EntityState{ get; private set; }
        public StorageSystem StorageSystem { get; protected set; }
        public EntityItemPreferences ItemPreferences { get; protected set; }
        

        public Entity(CommandInvoker invoker, EntitySO entitySo, StorageSystem storageSystem)
        {
            _invoker = invoker;
            EntityData = new EntityData(entitySo);
            EntityState = new EntityState(entitySo);
            StorageSystem = storageSystem;
        }
        
        public Entity()
        {
            
        }
        
    }
}