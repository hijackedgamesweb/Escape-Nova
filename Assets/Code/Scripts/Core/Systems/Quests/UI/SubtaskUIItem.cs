using TMPro;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.UI
{

    public class SubtaskUIItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text subtaskDescriptionText;
    
        // Asigna el Text(TMP) de tu prefab en el Inspector
        public void Setup(QuestObjective objective)
        {
            // ¡Esta es la línea clave que actualizamos!
            subtaskDescriptionText.text = objective.objectiveDescription;
        }
    }
}