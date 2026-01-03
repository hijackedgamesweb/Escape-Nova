using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class WarUIManager : MonoBehaviour
    {
        [Header("Paneles")]
        [SerializeField] private GameObject proposalPanel;      // Panel de "Aceptar/Rechazar"
        [SerializeField] private GameObject consequencePanel;   // Panel de "Has perdido recursos"


        private Civilization _currentAggressor;

        private void OnEnable()
        {
            // Nos suscribimos al evento estático de BaseBehaviour
            BaseBehaviour.OnWarDeclaredToPlayer += ShowProposal;
        }

        private void OnDisable()
        {
            BaseBehaviour.OnWarDeclaredToPlayer -= ShowProposal;
        }

        // 1. Se llama automáticamente cuando el Árbol o el Utility deciden guerra
        private void ShowProposal(Civilization aggressor)
        {
            _currentAggressor = aggressor;

            proposalPanel.SetActive(true);
            consequencePanel.SetActive(false); // Aseguramos que el otro está cerrado
        }

        // 2. Botón ACEPTAR (Luchar)
        public void OnAcceptWar()
        {
            if (_currentAggressor != null)
            {
                var behaviour = _currentAggressor.AIController as BaseBehaviour;
                if (behaviour != null)
                {
                    behaviour.StartWar();
                    Debug.Log("JUGADOR: Guerra aceptada. ¡A las armas!");
                }
            }
            proposalPanel.SetActive(false);
        }

        public void OnDeclineWar()
        {
            Debug.Log("JUGADOR: Guerra rechazada. Aplicando penalización de producción.");
        
            proposalPanel.SetActive(false);
            consequencePanel.SetActive(true);
        }
        public void OnCloseConsequences()
        {
            consequencePanel.SetActive(false);
        }
    }
}