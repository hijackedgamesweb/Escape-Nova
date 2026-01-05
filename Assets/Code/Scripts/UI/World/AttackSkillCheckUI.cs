using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.World
{
    public class AttackSkillCheckUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject container;
        [SerializeField] private GameObject tutorialPanel; // El panel explicativo
        [SerializeField] private RectTransform needle;     // La aguja que rota
        [SerializeField] private Image greenZone;          // Imagen "Filled" zona perfecta
        [SerializeField] private Image orangeZone;         // Imagen "Filled" zona 50%
        
        [Header("Settings")]
        [SerializeField] private float rotationSpeed = 200f;
        [Tooltip("Margen de 'gracia' para considerar que la aguja está dentro del fill amount")]
        [SerializeField] private float angleOffset = 0f; 

        private Action<SkillCheckResult> _onCompleteCallback;
        private bool _isRunning = false;
        private bool _isWaitingForTutorial = false;

        private const string PREF_TUTORIAL_SEEN = "FirstTimeAttack_Tutorial";

        public enum SkillCheckResult
        {
            Miss,
            Chance50,
            Hit100
        }

        private void Start()
        {
            if(container != null) container.SetActive(false);
            if(tutorialPanel != null) tutorialPanel.SetActive(false);
        }

        public void StartSkillCheck(Action<SkillCheckResult> onComplete)
        {
            _onCompleteCallback = onComplete;
            
            container.SetActive(true);
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
            _isWaitingForTutorial = true;
            tutorialPanel.SetActive(true);
            _isRunning = false; 
        }
        
        public void CloseTutorialAndStart()
        {
            PlayerPrefs.SetInt(PREF_TUTORIAL_SEEN, 1);
            PlayerPrefs.Save();
            
            tutorialPanel.SetActive(false);
            _isWaitingForTutorial = false;
            
            StartRotation();
        }

        private void StartRotation()
        {
            _isRunning = true;
            float randomStartAngle = UnityEngine.Random.Range(180f, 300f); 
            needle.localRotation = Quaternion.Euler(0, 0, randomStartAngle);
        }

        private void Update()
        {
            if (_isWaitingForTutorial) return;
            if (!_isRunning) return;
            needle.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.F))
            {
                CheckHit();
            }
        }

        private void CheckHit()
        {
            _isRunning = false;
            float currentZ = needle.localEulerAngles.z;
            float angle = (360f - currentZ) % 360f;
            float fillPos = angle / 360f;

            SkillCheckResult result = SkillCheckResult.Miss;
            
            if (fillPos <= greenZone.fillAmount)
            {
                result = SkillCheckResult.Hit100;
            }
            else if (fillPos <= orangeZone.fillAmount)
            {
                result = SkillCheckResult.Chance50;
            }
            else
            {
                result = SkillCheckResult.Miss;
            }
            container.SetActive(false);
            _onCompleteCallback?.Invoke(result);
        }
    }
}