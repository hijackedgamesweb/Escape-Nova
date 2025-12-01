using Code.Scripts.Core.Events;

namespace Code.Scripts.Core.Entity.Civilization
{
    public class CivilizationState : EntityState
    {
        public CivilizationState(CivilizationSO entitySO) : base(entitySO)
        {
            
        }

        public void SetCurrentMood(EntityMood mood)
        {
            CurrentMood = mood;
            UIEvents.OnUpdateCivilizationUI?.Invoke();
        }
    }
}