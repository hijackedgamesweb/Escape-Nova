using Code.Scripts.Camera;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI
{
    public class SunDirectionIndicator : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform _arrowIndicator;
        [SerializeField] private CameraController2D _cameraController;

        [Header("Settings")]
        [SerializeField] private float _edgePadding = 50f;

        [Header("HUD Settings")]
        [Tooltip("Altura en píxeles del HUD superior para evitar solapamiento")]
        [SerializeField] private float _topHudHeight = 100f;
        [Tooltip("Altura en píxeles del HUD inferior para evitar solapamiento")]
        [SerializeField] private float _bottomHudHeight = 100f;

        private Transform _sunTransform;
        private UnityEngine.Camera _mainCamera;
        private Canvas _parentCanvas;

        private void Start()
        {
            if (_cameraController != null)
                _mainCamera = _cameraController.GetComponent<UnityEngine.Camera>();
            else
                _mainCamera = UnityEngine.Camera.main;

            _parentCanvas = GetComponentInParent<Canvas>();
            if (_arrowIndicator != null) _arrowIndicator.gameObject.SetActive(false);
            FindSunReference();
        }

        private void FindSunReference()
        {
            var solarSystem = ServiceLocator.GetService<SolarSystem>();
            if (solarSystem != null)
            {
                foreach (Transform child in solarSystem.transform)
                {
                    _sunTransform = child;
                    break;
                }
            }
        }

        private void Update()
        {
            if (_mainCamera == null)
            {
                _mainCamera = UnityEngine.Camera.main;
                if (_mainCamera == null) return;
            }

            if (_sunTransform == null)
            {
                FindSunReference();
                return;
            }

            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            if (_mainCamera == null || _sunTransform == null || _arrowIndicator == null) return;

            Vector3 screenPos = _mainCamera.WorldToScreenPoint(_sunTransform.position);
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(_sunTransform.position);
            bool isOffScreen = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z < 0;

            if (isOffScreen)
            {
                if (!_arrowIndicator.gameObject.activeSelf) _arrowIndicator.gameObject.SetActive(true);

                Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
                Vector3 dir = (screenPos - screenCenter).normalized;
                if (viewportPos.z < 0)
                {
                    dir *= -1;
                }
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _arrowIndicator.rotation = Quaternion.Euler(0, 0, angle - 90);
                float safeHalfWidth = (Screen.width * 0.5f) - _edgePadding;
                float safeDistUp = (Screen.height * 0.5f) - _topHudHeight - _edgePadding;
                float safeDistDown = (Screen.height * 0.5f) - _bottomHudHeight - _edgePadding;
                float slopeX = Mathf.Abs(dir.x) > 0 ? safeHalfWidth / Mathf.Abs(dir.x) : float.MaxValue;

                float slopeY = float.MaxValue;
                if (dir.y > 0)
                {
                    slopeY = safeDistUp / dir.y;
                }
                else if (dir.y < 0)
                {
                    slopeY = safeDistDown / Mathf.Abs(dir.y);
                }
                float scaleFactor = Mathf.Min(slopeX, slopeY);
                Vector3 arrowPos = screenCenter + dir * scaleFactor;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentCanvas.transform as RectTransform,
                    arrowPos,
                    _parentCanvas.worldCamera,
                    out Vector2 localPoint);

                _arrowIndicator.anchoredPosition = localPoint;
            }
            else
            {
                if (_arrowIndicator.gameObject.activeSelf) _arrowIndicator.gameObject.SetActive(false);
            }
        }

        public void OnArrowClicked()
        {
            var mainCamera = UnityEngine.Camera.main;
            if (mainCamera != null && mainCamera.TryGetComponent<CameraController2D>(out var cameraController))
            {
                cameraController.ResetCameraOnTarget(_sunTransform);
            }
        }
    }
}