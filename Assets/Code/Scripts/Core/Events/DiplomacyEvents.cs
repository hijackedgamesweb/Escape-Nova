using System;
using Code.Scripts.Core.Entity.Civilization;

namespace Code.Scripts.Core.Events
{
    public static class DiplomacyEvents
    {
        public static Action<CivilizationData> OnWarWon;
        public static Action<CivilizationData> OnRelationshipDeclared;
        public static Action OnExchangeAgreed;
        public static Action<Civilization, bool> OnTradeProposed;
        public static Action OnCivilizationDiscovered;
    }
}