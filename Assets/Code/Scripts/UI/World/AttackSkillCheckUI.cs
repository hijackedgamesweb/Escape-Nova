using System;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.World
{
    public class AttackSkillCheckUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject container;
        [SerializeField] private GameObject tutorialPanel; 
        [SerializeField] private RectTransform needle;     
        [SerializeField] private Image greenZone;          
        [SerializeField] private Image orangeZone;         
        
        [Header("Settings")]
        [SerializeField] private float rotationSpeed = 200f;
        
        private Action<SkillCheckResult> _onCompleteCallback;
        private bool _isRunning = false;
        
        private float _accumulatedRotation = 0f;
        private IGameTime _gameTime;

        private const string PREF_TUTORIAL_SEEN = "FirstTimeAttack_Tutorial";

        public enum SkillCheckResult
        {
            Miss,       
            Chance50,   
            Hit100      
        }

        private void Awake()
        {
            if (_onCompleteCallback == null)
            {
                if(container != null) container.SetActive(false);
                if(tutorialPanel != null) tutorialPanel.SetActive(false);
            }
        }

        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
        }

        public void StartSkillCheck(Action<SkillCheckResult> onComplete)
        {
            _onCompleteCallback = onComplete;
            this.gameObject.SetActive(true);
            if(container != null) container.SetActive(true);
            if (PlayerPrefs.GetInt(PREF_TUTORIAL_SEEN, 0) == 0)
            {
                ShowTutorial();
            }
            else
            {
                StartRotation();
            }
        }

        private void ShowTutorial()
        {
            if(tutorialPanel != null) tutorialPanel.SetActive(true);
            if (_gameTime != null) _gameTime.Pause();
            _isRunning = false; 
        }
        
        public void CloseTutorialAndStart()
        {
            PlayerPrefs.SetInt(PREF_TUTORIAL_SEEN, 1);
            PlayerPrefs.Save();
            if(tutorialPanel != null) tutorialPanel.SetActive(false);
            if (_gameTime != null) _gameTime.Resume();
            StartRotation();
        }

        private void StartRotation()
        {
            _isRunning = true;
            _accumulatedRotation = 0f;
            float randomStartAngle = UnityEngine.Random.Range(180f, 300f); 
            if(needle != null) needle.localRotation = Quaternion.Euler(0, 0, randomStartAngle);
        }

        private void Update()
        {
            if (!_isRunning) return;

            if (needle != null)
            {
                float step = rotationSpeed * Time.deltaTime;
                
                needle.Rotate(0, 0, -step);
                _accumulatedRotation += step;
                if (_accumulatedRotation >= 360f)
                {
                    _isRunning = false;
                    Finish(SkillCheckResult.Miss);
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                CheckHit();
            }
        }

        private void CheckHit()
        {
            _isRunning = false;
            
            if (needle == null) 
            {
                Finish(SkillCheckResult.Miss);
                return;
            }

            float currentZ = needle.localEulerAngles.z;
            float angle = (360f - currentZ) % 360f; 
            float fillPos = angle / 360f;

            SkillCheckResult result = SkillCheckResult.Miss;

            float greenLimit = (greenZone != null) ? greenZone.fillAmount : 0.1f;
            float orangeLimit = (orangeZone != null) ? orangeZone.fillAmount : 0.25f;

            if (fillPos <= greenLimit)
            {
                result = SkillCheckResult.Hit100;
            }
            else if (fillPos <= orangeLimit)
            {
                result = SkillCheckResult.Chance50;
            }
            else
            {
                result = SkillCheckResult.Miss;
            }

            Finish(result);
        }

        private void Finish(SkillCheckResult result)
        {
            if(container != null) container.SetActive(false);
            
            var callback = _onCompleteCallback;
            _onCompleteCallback = null; 
            this.gameObject.SetActive(false);

            callback?.Invoke(result);
        }

        [ContextMenu("Reset Tutorial Pref")]
        public void ResetTutorialPref()
        {
            PlayerPrefs.DeleteKey(PREF_TUTORIAL_SEEN);
            PlayerPrefs.Save();
        }
    }
}