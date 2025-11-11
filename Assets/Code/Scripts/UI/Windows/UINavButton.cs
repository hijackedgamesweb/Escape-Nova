using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Code.Scripts.UI.Windows
{
    [RequireComponent(typeof(Button))]
    public class UINavButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        [Header("Manager")]
        [Tooltip("Arrastra aquí el script MainMenuScreen que gestiona este panel")]
        public MainMenuScreen menuManager;
    
        private Button _button;
    
        void Awake()
        {
            _button = GetComponent<Button>();
        }
    
        /// <summary>
        /// Se llama CADA VEZ que el ratón entra en este botón.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Si el botón no está desactivado (interactable)...
            if (_button.IsInteractable())
            {
                // ...le decimos al EventSystem que este es el nuevo objeto seleccionado.
                // Esto quita el foco del botón anterior (ej: "Jugar").
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
        }
    
        /// <summary>
        /// Se llama CADA VEZ que este botón es seleccionado (por ratón O teclado).
        /// </summary>
        public void OnSelect(BaseEventData eventData)
        {
            // Le avisamos al panel principal que AHORA somos el último botón seleccionado.
            if (menuManager != null)
            {
                menuManager.SetLastSelectedButton(this.gameObject);
            }
        }
    }
}