using UnityEngine;

namespace Code.Scripts.Core.Entity.Player
{
    [CreateAssetMenu(fileName = "New PlayerSO", menuName = "Core/Entity/PlayerSO")]
    public class PlayerSO : EntitySO
    {
        public float baseWood;
        public float baseFood;
    }
}