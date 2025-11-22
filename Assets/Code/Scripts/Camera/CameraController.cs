using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus.States.GameStates;
using Code.Scripts.UI.Windows;
using DG.Tweening;
using UnityEngine;

namespace Code.Scripts.Camera
{
    public class CameraController2D : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] float _zoomSpeed = 1f;
        [SerializeField] float _minZoom = 2f;
        [SerializeField] float _maxZoom = 10f;
        
        [Header("Movement Settings")]
        [SerializeField] float _moveSpeed = 5f;
        [SerializeField] float _inertiaDamping = 5f;
        
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private GameObject _solarSystem;
        
        [Header("Parallax Settings")]
        [SerializeField, Range(0f, 1f)] float _backgroundParallax = 0.3f;
        [SerializeField, Range(0f, 1f)] float _solarParallax = 0.8f;
        
        private Vector3 _initialBackgroundPos;
        private Vector3 _initialSolarPos;
        private Vector3 _initialCameraPos;
        
        private UnityEngine.Camera _mainCamera;
        private Vector3 _dragOrigin;
        private bool _isDragging = false;
        
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _lastMousePosition;
        
        private Tween _zoomTween;
        private Tween _moveTween;
        
        private Transform _target;
        void Start()
        {
            _mainCamera = GetComponent<UnityEngine.Camera>();
            
            if(_background != null)
                _initialBackgroundPos = _background.transform.position;
            if(_solarSystem != null)
                _initialSolarPos = _solarSystem.transform.position;
            _initialCameraPos = transform.position;
        }

        void Update()
        {
            if(UIManager.Instance.GetCurrentScreen() is not InGameScreen) return;
            HandleZoom();
            HandleMovement();
            HandleFollowTarget();

            ApplyParallax();
        }

        private void ApplyParallax()
        {
            if(_background != null)
            {
                Vector3 backgroundTargetPos = _initialBackgroundPos + (transform.position - _initialCameraPos) * _backgroundParallax;
                _background.transform.position = backgroundTargetPos;
            }
            if(_solarSystem != null)
            {
                Vector3 solarTargetPos = _initialSolarPos + (transform.position - _initialCameraPos) * _solarParallax;
                _solarSystem.transform.position = solarTargetPos;
            }
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                _zoomTween?.Kill();
                _moveTween?.Kill(); 

                Vector3 mouseWorldPosBeforeZoom = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                
                float newSize = _mainCamera.orthographicSize - scroll * _zoomSpeed;
                _mainCamera.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);
                
                Vector3 mouseWorldPosAfterZoom = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                transform.position += mouseWorldPosBeforeZoom - mouseWorldPosAfterZoom;
            }

            ClampCameraToBounds();
        }
        
        private void HandleMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDragging = true;
                _dragOrigin = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                _lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }

            if (_isDragging)
            {
                _moveTween?.Kill();
                
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 difference = _mainCamera.ScreenToWorldPoint(currentMousePosition) - _mainCamera.ScreenToWorldPoint(_lastMousePosition);
                transform.position -= difference;
                _lastMousePosition = currentMousePosition;
                
                _velocity = difference / Time.deltaTime;
            }
            else
            {
                if (_velocity.magnitude > 0.1f)
                {
                    transform.position -= _velocity * Time.deltaTime;
                    _velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.deltaTime * _inertiaDamping);
                }
                else
                {
                    _velocity = Vector3.zero;
                }
            }

            ClampCameraToBounds();
        }
        
        private void HandleFollowTarget()
        {
            if (_target != null)
            {
                Vector3 targetPosition = new Vector3(_target.position.x, _target.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _moveSpeed);
            }
        }
        
        private void ClampCameraToBounds()
        {
            if (_background == null) return;

            Bounds bounds = _background.bounds;

            float cameraHalfHeight = _mainCamera.orthographicSize;
            float cameraHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;

            float minX = bounds.min.x + cameraHalfWidth;
            float maxX = bounds.max.x - cameraHalfWidth;
            float minY = bounds.min.y + cameraHalfHeight;
            float maxY = bounds.max.y - cameraHalfHeight;

            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
            transform.position = clampedPos;
        }
        
        public void SetTarget(Transform target)
        {
            _zoomTween?.Kill();
            _moveTween?.Kill();

            _target = target;

            Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);

            _moveTween = transform.DOMove(targetPos, 0.8f)
                .SetEase(Ease.OutCirc);

            _zoomTween = DOTween.To(
                () => _mainCamera.orthographicSize,
                x => _mainCamera.orthographicSize = x,
                _minZoom,
                0.8f
            ).SetEase(Ease.OutCirc);
        }

        public void ClearTarget()
        {
            _target = null;
            _zoomTween?.Kill();
            _zoomTween = DOTween.To(
                () => _mainCamera.orthographicSize,
                x => _mainCamera.orthographicSize = x,
                _maxZoom,
                1f
            ).SetEase(Ease.OutCirc);
        }

        public void ResetDragState()
        {
            _isDragging = false;
            _velocity = Vector3.zero;
        }
    }
}