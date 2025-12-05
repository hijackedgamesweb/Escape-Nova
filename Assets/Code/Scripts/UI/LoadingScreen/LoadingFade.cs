using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.LoadingScreen
{
    public class LoadingFade : MonoBehaviour
    {
        public Image fadeImage;
        public float fadeDuration = 0.5f;

        void Awake()
        {
            StartCoroutine(FadeIn());
        }

        IEnumerator FadeIn()
        {
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, 1 - (t / fadeDuration));
                yield return null;
            }
        }
    }
}