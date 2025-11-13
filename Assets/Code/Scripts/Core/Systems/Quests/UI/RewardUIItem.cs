using Code.Scripts.Core.Systems.Quests.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.UI
{
    public class RewardUIItem : MonoBehaviour
    {
        // Asigna el componente de texto de tu prefab aquí
        [SerializeField] private TMP_Text rewardText;
    
        public void Setup(QuestReward reward)
        {
            // Mostramos "Descripción: Cantidad" (e.g., "Oro: 100")
            rewardText.text = $"{reward.Description}: {reward.Amount}";
        }
    }
}