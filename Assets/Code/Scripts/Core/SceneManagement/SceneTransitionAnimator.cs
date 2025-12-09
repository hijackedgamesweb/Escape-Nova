using Code.Scripts.Core.SaveLoad;
using Code.Scripts.Patterns.Singleton;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionAnimator : Singleton<SceneTransitionAnimator>
{
    public RectTransform rocket;
    public RectTransform trailFill;
    public float rocketDuration = 1.2f;
    public float fillDuration = 0.7f;

    private Vector2 leftOffscreen;
    private Vector2 rightOffscreen;

    void Start()
    {
    }

    public async void Play(string sceneName)
    {
        Sequence s = DOTween.Sequence();

        // 1) La nave entra por la izquierda hacia el centro
        s.Append(rocket.DOAnchorPosX(2908, rocketDuration).SetEase(Ease.OutQuad));

        // 3) Cambio de escena cuando la pantalla está cubierta
        s.AppendCallback(() =>
        {
            SceneManager.LoadScene(sceneName);
        });

        // 5) Nave sale por la derecha
        s.Append(
            rocket.DOAnchorPosX(7000, rocketDuration)
                .SetEase(Ease.InQuad)
        );
        
        // Al acabar vuelve a la posicion oriniginal
        s.AppendCallback(() =>
        {
            rocket.anchoredPosition = new Vector2(-147, rocket.anchoredPosition.y);
        });
        
    }
}