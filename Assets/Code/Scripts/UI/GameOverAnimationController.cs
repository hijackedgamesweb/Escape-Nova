using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.Scripts.UI
{
    public class GameOverAnimationController : MonoBehaviour
    {
        [Header("Referencias de UI")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private GameObject gameOverTextObject;

        [Header("Configuración de Animación")]
        [SerializeField] private float fadeDuration = 1.5f;
        public void StartGameOverSequence()
        {
            StartCoroutine(PlayAnimation());
        }

        private IEnumerator PlayAnimation()
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySFX("GameOverSFX");
            if (gameOverTextObject != null)
            {
                gameOverTextObject.SetActive(false);
            }

            Color imageColor = fadeImage.color;
            imageColor.a = 0f;
            fadeImage.color = imageColor;
            
            gameObject.SetActive(true);
            float timer = 0f;
            while (timer < fadeDuration)
            {
                float newAlpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                
                imageColor.a = newAlpha;
                fadeImage.color = imageColor;
                
                timer += Time.deltaTime;
                yield return null;
            }

            imageColor.a = 1f;
            fadeImage.color = imageColor;
            if (gameOverTextObject != null)
            {
                gameOverTextObject.SetActive(true);
            }
        }
    }
}