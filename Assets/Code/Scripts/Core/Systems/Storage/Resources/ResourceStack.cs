using System;

namespace Code.Scripts.Core.Systems.Resources
{
    [Serializable]
    public struct ResourceStack
    {
        public ResourceType Type;
        public int Amount;
        
        public ResourceStack(ResourceType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
        
        public override string ToString()
        {
            return $"{Amount} {Type}";
        }
    }
}