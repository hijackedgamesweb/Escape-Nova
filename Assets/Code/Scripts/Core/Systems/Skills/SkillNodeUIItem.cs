using UnityEngine;
using UnityEngine.UI;
using Code.Scripts.Core.Systems.Skills;
using TMPro;
using System.Collections;

namespace Code.Scripts.UI.Skills
{
    public class SkillNodeUIItem : MonoBehaviour
    {
        [SerializeField] private Button nodeButton;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image glowEffect;
        [SerializeField] private Image purchasedIndicator; // Nueva: indicador de comprado
        [SerializeField] private TextMeshProUGUI nodeNameText;

        [Header("Colors")]
        [SerializeField] private Color purchasedColor = Color.gray;
        [SerializeField] private Color unlockedColor = Color.green;
        [SerializeField] private Color lockedColor = Color.red;

        [Header("Glow Effects")]
        [SerializeField] private bool enablePulse = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float unlockedPulseIntensity = 1.2f;
        [SerializeField] private float purchasedPulseIntensity = 1.1f;

        private SkillNodeData nodeData;
        private SkillTreeManager skillTreeManager;
        private SkillTreeUI skillTreeUI;
        private Vector3 originalScale;
        private Coroutine pulseCoroutine;
        private Color constellationColor = Color.white;

        // Método para establecer el color de la constelación
        public void SetConstellationColor(Color color)
        {
            constellationColor = color;

            // Aplicar inmediatamente si el nodo ya está configurado
            if (nodeData != null)
            {
                UpdateVisualState();
            }
        }

        public void Initialize(SkillNodeData data, SkillTreeManager manager, SkillTreeUI ui)
        {
            nodeData = data;
            skillTreeManager = manager;
            skillTreeUI = ui;

            // Configurar nombre del nodo
            if (nodeNameText != null && nodeData != null)
            {
                nodeNameText.text = nodeData.nodeName;
            }

            if (nodeButton != null)
            {
                nodeButton.onClick.RemoveAllListeners();
                nodeButton.onClick.AddListener(OnNodeClicked);
            }
            else
            {
                Debug.LogError("SkillNodeUIItem: NodeButton is not assigned!");
            }

            // Guardar escala original
            originalScale = transform.localScale;

            // Ocultar glow inicialmente si no está configurado
            if (glowEffect != null)
            {
                glowEffect.gameObject.SetActive(false);
            }

            // Ocultar indicador de comprado inicialmente
            if (purchasedIndicator != null)
            {
                purchasedIndicator.gameObject.SetActive(false);
            }

            UpdateVisualState();
        }

        private void OnNodeClicked()
        {
            if (skillTreeUI != null && nodeData != null)
            {
                skillTreeUI.ShowNodeModal(nodeData);
            }
        }

        public void UpdateVisualState()
        {
            if (nodeData == null || skillTreeManager == null || backgroundImage == null || nodeButton == null)
            {
                Debug.LogWarning("SkillNodeUIItem: Missing references for visual state update");
                return;
            }

            try
            {
                if (skillTreeManager.IsSkillPurchased(nodeData))
                {
                    // NODO COMPRADO: Color fijo sin pulso
                    backgroundImage.color = purchasedColor;
                    nodeButton.interactable = true; // Sigue siendo clickeable para mostrar info

                    // DETENER TODOS LOS EFECTOS DE PULSO
                    if (pulseCoroutine != null)
                    {
                        StopCoroutine(pulseCoroutine);
                        pulseCoroutine = null;
                    }

                    // Escala fija, sin animación
                    transform.localScale = originalScale;

                    // Ocultar glow de pulso
                    if (glowEffect != null)
                    {
                        glowEffect.gameObject.SetActive(false);
                    }

                    // Mostrar indicador de comprado (checkmark o similar)
                    if (purchasedIndicator != null)
                    {
                        purchasedIndicator.gameObject.SetActive(true);
                        purchasedIndicator.color = Color.white;
                    }
                }
                else if (skillTreeManager.IsSkillUnlocked(nodeData))
                {
                    // Nodo desbloqueado pero no comprado: color de la constelación con pulso
                    backgroundImage.color = Color.Lerp(unlockedColor, constellationColor, 0.3f);
                    nodeButton.interactable = true;

                    // Iniciar efecto de pulso
                    if (enablePulse && gameObject.activeInHierarchy)
                    {
                        if (pulseCoroutine != null)
                        {
                            StopCoroutine(pulseCoroutine);
                        }
                        pulseCoroutine = StartCoroutine(PulseEffect(unlockedPulseIntensity, constellationColor));
                    }

                    // Mostrar glow
                    if (glowEffect != null)
                    {
                        glowEffect.gameObject.SetActive(true);
                        glowEffect.color = constellationColor;
                    }

                    // Ocultar indicador de comprado
                    if (purchasedIndicator != null)
                    {
                        purchasedIndicator.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // Nodo bloqueado
                    backgroundImage.color = lockedColor;
                    nodeButton.interactable = false;

                    // Detener efectos
                    if (pulseCoroutine != null)
                    {
                        StopCoroutine(pulseCoroutine);
                        pulseCoroutine = null;
                    }
                    transform.localScale = originalScale;

                    // Ocultar glow
                    if (glowEffect != null)
                    {
                        glowEffect.gameObject.SetActive(false);
                    }

                    // Ocultar indicador de comprado
                    if (purchasedIndicator != null)
                    {
                        purchasedIndicator.gameObject.SetActive(false);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error updating visual state for node {nodeData.nodeName}: {e.Message}");
                backgroundImage.color = lockedColor;
                nodeButton.interactable = false;
            }
        }

        private IEnumerator PulseEffect(float intensity, Color pulseColor)
        {
            // Efecto de pulso solo para nodos desbloqueados (no comprados)
            while (true)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
                float scale = Mathf.Lerp(1f, intensity, pulse);
                transform.localScale = originalScale * scale;

                // Pulso de glow
                if (glowEffect != null && glowEffect.gameObject.activeSelf)
                {
                    Color glowColor = glowEffect.color;
                    glowColor.a = Mathf.Lerp(0.3f, 0.7f, pulse);
                    glowEffect.color = glowColor;
                }

                yield return null;
            }
        }

        // Efecto especial al comprar (sin pulso posterior)
        public void PlayPurchaseEffect()
        {
            StartCoroutine(PurchaseEffectCoroutine());
        }

        private IEnumerator PurchaseEffectCoroutine()
        {
            // Flash de compra
            Image image = GetComponent<Image>();
            Color originalColor = image.color;

            // Flash blanco rápido
            image.color = Color.white;
            yield return new WaitForSeconds(0.1f);

            // Vuelta al color original
            image.color = originalColor;

            // Efecto de "explosión" o "destello" sin pulso
            Vector3 startScale = originalScale * 1.5f;
            transform.localScale = startScale;

            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;

            // Después del efecto, actualizar el estado visual para asegurar que no hay pulso
            UpdateVisualState();
        }

        // Método para forzar la detención del pulso (útil cuando se compra desde fuera)
        public void StopAllEffects()
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }

            transform.localScale = originalScale;

            if (glowEffect != null)
            {
                glowEffect.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // Reanudar efectos solo si no está comprado
            if (nodeData != null && skillTreeManager != null)
            {
                UpdateVisualState();
            }
        }

        private void OnDisable()
        {
            // Detener efectos cuando el objeto se desactiva
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
        }

        private void OnDestroy()
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
            }
        }
    }
}