using UnityEngine;
using System.Collections;

namespace Code.Scripts.UI.Skills
{
    public class ConstellationRevealAnimation : MonoBehaviour
    {
        [SerializeField] private float revealDuration = 1.5f;
        [SerializeField] private AnimationCurve revealCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private CanvasGroup[] nodeCanvasGroups;
        private UILineRenderer[] lineRenderers;

        public void Initialize(Transform[] nodes, UILineRenderer[] lines)
        {
            nodeCanvasGroups = new CanvasGroup[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                var canvasGroup = nodes[i].gameObject.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                nodeCanvasGroups[i] = canvasGroup;
            }

            lineRenderers = lines;
            foreach (var line in lineRenderers)
            {
                line.color = new Color(line.color.r, line.color.g, line.color.b, 0);
            }
        }

        public IEnumerator RevealConstellation()
        {
            float timer = 0f;

            while (timer < revealDuration)
            {
                float progress = timer / revealDuration;
                float curveValue = revealCurve.Evaluate(progress);

                // Animar nodos
                foreach (var canvasGroup in nodeCanvasGroups)
                {
                    canvasGroup.alpha = curveValue;
                }

                // Animar líneas
                foreach (var line in lineRenderers)
                {
                    line.color = new Color(line.color.r, line.color.g, line.color.b, curveValue * 0.5f);
                }

                timer += Time.deltaTime;
                yield return null;
            }

            // Asegurar que todo está visible al final
            foreach (var canvasGroup in nodeCanvasGroups)
            {
                canvasGroup.alpha = 1f;
            }

            foreach (var line in lineRenderers)
            {
                line.color = new Color(line.color.r, line.color.g, line.color.b, 0.5f);
            }
        }
    }
}