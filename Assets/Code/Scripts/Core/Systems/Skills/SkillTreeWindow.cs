using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.UI.Skills
{
    public class SkillTreeWindow : MonoBehaviour
    {
        [SerializeField] private GameObject skillTreePanel;
        private bool isVisible = false;

        private void Start()
        {
            // Inicialmente oculto
            if (skillTreePanel != null)
            {
                skillTreePanel.SetActive(false);
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
                    var skillTreeUI = skillTreePanel.GetComponent<SkillTreeUI>();
                    if (skillTreeUI != null)
                    {
                        skillTreeUI.RefreshUI();
                    }
                }
            }
        }

        // Métodos públicos para botones de UI si los necesitas
        public void ShowSkillTree()
        {
            isVisible = true;
            skillTreePanel.SetActive(true);
            var skillTreeUI = skillTreePanel.GetComponent<SkillTreeUI>();
            if (skillTreeUI != null)
            {
                skillTreeUI.RefreshUI();
            }
        }

        public void HideSkillTree()
        {
            isVisible = false;
            skillTreePanel.SetActive(false);
        }
    }
}