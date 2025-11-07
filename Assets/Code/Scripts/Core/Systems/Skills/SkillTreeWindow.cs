using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Systems.Skills;

namespace Code.Scripts.UI.Skills
{
    public class SkillTreeWindow : MonoBehaviour
    {
        [SerializeField] private GameObject skillTreePanel;
        private bool isVisible = false;
        private SkillTreeUI skillTreeUI;

        private void Start()
        {
            // Inicialmente oculto
            if (skillTreePanel != null)
            {
                skillTreePanel.SetActive(false);
                skillTreeUI = skillTreePanel.GetComponent<SkillTreeUI>();
            }
        }

        private void Update()
        {
            // Detectar tecla C para abrir/cerrar
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleSkillTree();
            }
        }

        public void ToggleSkillTree()
        {
            isVisible = !isVisible;

            if (skillTreePanel != null)
            {
                skillTreePanel.SetActive(isVisible);

                // Si se muestra, actualizar la UI
                if (isVisible)
                {
                    if (skillTreeUI != null)
                    {
                        skillTreeUI.RefreshUI();
                    }
                }
                else
                {
                    // Si se cierra el menú, cerrar también el modal
                    if (skillTreeUI != null)
                    {
                        skillTreeUI.HideModal();
                    }
                }
            }
        }

        // Métodos públicos para botones de UI si los necesitas
        public void ShowSkillTree()
        {
            isVisible = true;
            skillTreePanel.SetActive(true);
            if (skillTreeUI != null)
            {
                skillTreeUI.RefreshUI();
            }
        }

        public void HideSkillTree()
        {
            isVisible = false;
            skillTreePanel.SetActive(false);

            // Cerrar también el modal al ocultar el árbol
            if (skillTreeUI != null)
            {
                skillTreeUI.HideModal();
            }
        }
    }
}