using Code.Scripts.Core.Entity;
using Code.Scripts.Core.Entity.Player;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Newtonsoft.Json.Linq;

namespace Code.Scripts.Player
{
    public class PlayerState : EntityState
    {
        public PlayerState(PlayerSO entitySO) : base(entitySO)
        {
        }

    }
}