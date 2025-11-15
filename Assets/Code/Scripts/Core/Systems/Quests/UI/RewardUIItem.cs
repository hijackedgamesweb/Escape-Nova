using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.UI
{
    public class RewardUIItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text rewardText;
    
        public void Setup(QuestReward reward)
        {
            rewardText.text = reward.GetRewardInfo();
        }
    }
}