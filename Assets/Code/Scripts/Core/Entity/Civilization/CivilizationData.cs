using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Entity.Civilization
{
    public class CivilizationData : EntityData
    {
        public string CivilizationDescription;
        public Sprite CivilizationIcon;
        public Sprite CivilizationFlag;
        
        public Sprite LeaderPortrait;

        public float AngerTolerance;

        public CivilizationData(CivilizationSO entitySO) : base(entitySO)
        {
            CivilizationDescription = entitySO.civilizationDescription;
            CivilizationIcon = entitySO.civilizationIcon;
            CivilizationFlag = entitySO.civilizationFlag;
            LeaderPortrait = entitySO.leaderPortrait;
            AngerTolerance = entitySO.angerTolerance;
        }
    }
}