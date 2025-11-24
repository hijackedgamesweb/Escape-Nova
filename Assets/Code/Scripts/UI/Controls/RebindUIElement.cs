using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace Code.Scripts.UI.Controls
{
    public class RebindUIElement : MonoBehaviour
    {
        [Header("Configuración")]
        [Tooltip("El nombre exacto de la Acción en tu Input Action Asset (ej: 'Jump', 'Interact')")]
        [SerializeField] private string actionName;
        [Tooltip("El índice del binding. Generalmente 0 para teclado/ratón. Si tienes varios (ej: flechas Y WASD), tendrás que buscar el índice correcto.")]
        [SerializeField] private int bindingIndex = 0;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Button rebindButton;

        private InputAction _inputAction;

        private void Start()
        {
            _inputAction = RebindMenuManager.Instance.GetAction(actionName);

            if (_inputAction == null)
            {
                Debug.LogError($"RebindUIElement: No se encontró la acción '{actionName}'");
                return;
            }
            
            rebindButton.onClick.AddListener(() => DoRebind());
            UpdateUI();
            RebindMenuManager.Instance.OnRebindComplete += UpdateUI;
        }

        private void OnDestroy()
        {
             if (RebindMenuManager.Instance != null)
             {
                 RebindMenuManager.Instance.OnRebindComplete -= UpdateUI;
             }
        }

        public void UpdateUI()
        {
            if (_inputAction != null)
            {
                string displayString = _inputAction.GetBindingDisplayString(bindingIndex, 
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
        
                buttonText.text = displayString.ToUpper();
            }
        }

        private void DoRebind()
        {
            RebindMenuManager.Instance.StartRebindProcess(actionName, bindingIndex, buttonText);
        }
    }
}