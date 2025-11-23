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
        private Transform _sunTransform;
        private UnityEngine.Camera _mainCamera;
        private Canvas _parentCanvas;

        private void Start()
        {
            _mainCamera = _cameraController.GetComponent<UnityEngine.Camera>();
            _parentCanvas = GetComponentInParent<Canvas>();
            _arrowIndicator.gameObject.SetActive(false);
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
            if (_sunTransform == null)
            {
                FindSunReference();
                return;
            }

            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
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

                float halfWidth = (Screen.width * 0.5f) - _edgePadding;
                float halfHeight = (Screen.height * 0.5f) - _edgePadding;

                float slopeX = Mathf.Abs(dir.x) > 0 ? halfWidth / Mathf.Abs(dir.x) : float.MaxValue;
                float slopeY = Mathf.Abs(dir.y) > 0 ? halfHeight / Mathf.Abs(dir.y) : float.MaxValue;

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