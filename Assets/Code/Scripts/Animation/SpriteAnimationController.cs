using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEditor;

namespace Code.Scripts.Animation
{
    using UnityEngine;

    public class SpriteAnimatorController : MonoBehaviour
    {
        [Header("Rendering")]
        public SpriteRenderer spriteRenderer;

        [Header("Animation Data")]
        public PlanetDataSO animationData;

        private IGameTime _gameTime;
        private Sprite[] frames;
        private float frameRate;
        private int currentFrame;
        private float timer;

        void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            if (animationData != null)
                LoadAnimation(animationData);
        }

        void Update()
        {
            if (_gameTime == null || _gameTime.IsPaused || frames == null || frames.Length == 0)
                return;

            timer += Time.deltaTime * _gameTime.TimeScale;

            if (timer >= frameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % frames.Length;
                spriteRenderer.sprite = frames[currentFrame];
            }
        }

        public void LoadAnimation(PlanetDataSO newAnimation)
        {
            animationData = newAnimation;
            frames = animationData.animationFrames;
            frameRate = animationData.frameRate;
            currentFrame = 0;

            if (frames != null && frames.Length > 0)
                spriteRenderer.sprite = frames[0];
        }
    }

}