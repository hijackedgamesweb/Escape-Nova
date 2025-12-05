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
        [SerializeField] private Color unlockedColor = Color.green;
        [SerializeField] private Color lockedColor = Color.red;
        [SerializeField] private Color purchasedDimColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Color atenuado para nodos comprados

        [Header("Glow Effects")]
        [SerializeField] private bool enablePulse = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float unlockedPulseIntensity = 1.2f;

        private SkillNodeData nodeData;
        private SkillTreeManager skillTreeManager;
        private SkillTreeUI skillTreeUI;
        private Vector3 originalScale;
        private Coroutine pulseCoroutine;
        private Color constellationColor = Color.white;
        private Color purchasedColor = Color.white; // Color de comprado basado en constelación

        // Método para establecer el color de la constelación
        public void SetConstellationColor(Color color)
        {
            constellationColor = color;

            // Calcular color para nodos comprados (versión más oscura del color de constelación)
            purchasedColor = new Color(
                color.r * 0.6f,  // 60% del brillo
                color.g * 0.6f,
                color.b * 0.6f,
                1f
            );

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
                    // NODO COMPRADO: Color de constelación atenuado sin pulso
                    backgroundImage.color = purchasedColor; // Usar versión oscura del color de constelación
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

                    // Mostrar indicador de comprado (checkmark o similar) en blanco para contraste
                    if (purchasedIndicator != null)
                    {
                        purchasedIndicator.gameObject.SetActive(true);
                        purchasedIndicator.color = Color.white;
                    }

                    // Asegurar que el nombre del nodo sea visible (blanco o color claro)
                    if (nodeNameText != null)
                    {
                        nodeNameText.color = Color.white;
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

                    // Restaurar color original del texto
                    if (nodeNameText != null)
                    {
                        nodeNameText.color = Color.white;
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

                    // Color de texto para nodos bloqueados (más oscuro)
                    if (nodeNameText != null)
                    {
                        nodeNameText.color = new Color(0.3f, 0.3f, 0.3f, 1f);
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
            // Flash de compra usando el color de constelación original (no atenuado)
            Image image = GetComponent<Image>();
            Color originalImageColor = image.color;

            // Flash rápido con el color brillante de la constelación
            image.color = new Color(constellationColor.r * 1.5f, constellationColor.g * 1.5f, constellationColor.b * 1.5f, 1f);
            yield return new WaitForSeconds(0.1f);

            // Transición suave al color de comprado (atenuado)
            float duration = 0.4f;
            float elapsed = 0f;
            Color startColor = image.color;
            Color targetColor = purchasedColor;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                image.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            image.color = targetColor;

            // Efecto de "explosión" sutil
            Vector3 startScale = originalScale * 1.3f;
            transform.localScale = startScale;

            elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.3f;
                transform.localScale = Vector3.Lerp(startScale, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;

            // Asegurar que el estado visual está actualizado
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