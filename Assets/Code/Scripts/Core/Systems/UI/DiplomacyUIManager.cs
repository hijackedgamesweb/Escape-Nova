using UnityEngine;

namespace Code.Scripts.Core.Systems.UI
{
    public class DiplomacyUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject proposalPanel;
        public void OpenProposalPanel()
        {
            proposalPanel.SetActive(true);
        }
    }
}