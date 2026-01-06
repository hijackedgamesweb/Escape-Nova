using System.Collections;
using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Windows;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch; // Necesario para el sistema tactil nuevo
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch; // Alias para evitar conflictos

namespace Code.Scripts.Camera
{
    public class CameraController2D : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] float _zoomSpeed = 1f;
        [SerializeField] float _minZoom = 2f;
        [SerializeField] float _maxZoom = 10f;
        [SerializeField] float _unFocusMaxZoom = 5f;
        
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
        private Vector3 _lastMousePosition;
        private Vector3 _dragOrigin;
        private bool _isDragging = false;
        private bool _isTweening = false;
        private bool _inputBlocked = false;
        private Vector3 _velocity = Vector3.zero;
        
        private Tween _zoomTween;
        private Tween _moveTween;
        private Transform _target;

        void Awake()
        {
            _mainCamera = GetComponent<UnityEngine.Camera>();
            ServiceLocator.RegisterService<CameraController2D>(this);
        }

        void Start()
        {
            if(_background != null)
                _initialBackgroundPos = _background.transform.position;
            if(_solarSystem != null)
                _initialSolarPos = _solarSystem.transform.position;
            _initialCameraPos = transform.position;
        }
        
        void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }
        
        void OnDisable()
        {
            EnhancedTouchSupport.Disable();
            _zoomTween?.Kill();
            _moveTween?.Kill();
        }

        void OnDestroy()
        {
            ServiceLocator.UnregisterService<CameraController2D>();
        }

        void Update()
        {
            if(UIManager.Instance.GetCurrentScreen() is not InGameScreen) return;
            if (_isTweening || _inputBlocked) return;
            var touches = Touch.activeTouches;

            if (touches.Count > 0)
            {
                HandleMobileInput(touches);
            }
            else
            {
                HandleMouseZoom();
                HandleMouseMovement();
            }
            HandleFollowTarget();
            ClampCameraToBounds();
            ApplyParallax();
        }

        private void HandleMobileInput(UnityEngine.InputSystem.Utilities.ReadOnlyArray<Touch> touches)
        {
            if (touches.Count == 2)
            {
                _isDragging = false;
                _velocity = Vector3.zero;
                ClearTarget();
                
                HandleTouchZoom(touches[0], touches[1]);
            }
            else if (touches.Count == 1)
            {
                Touch touch = touches[0];
                if (IsPointerOverUI(touch.touchId)) 
                {
                    _isDragging = false;
                    return;
                }

                HandleTouchPan(touch);
            }
        }

        private void HandleTouchPan(Touch touch)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                _dragOrigin = _mainCamera.ScreenToWorldPoint(touch.screenPosition);
                _isDragging = true;
                _velocity = Vector3.zero;
                ClearTarget();
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && _isDragging)
            {
                Vector3 currentPos = _mainCamera.ScreenToWorldPoint(touch.screenPosition);
                Vector3 difference = _dragOrigin - currentPos;
                transform.position += difference;
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                _isDragging = false;
            }
        }

        private void HandleTouchZoom(Touch touch0, Touch touch1)
        {
            Vector2 touch0PrevPos = touch0.screenPosition - touch0.delta;
            Vector2 touch1PrevPos = touch1.screenPosition - touch1.delta;
            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.screenPosition - touch1.screenPosition).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float newSize = _mainCamera.orthographicSize + (deltaMagnitudeDiff * _zoomSpeed * 0.01f);
            _mainCamera.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);
        }

        private void HandleMouseZoom()
        {
            if (IsPointerOverUI(-1)) return;

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
        }
        
        private void HandleMouseMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUI(-1)) 
                {
                    return;
                }
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

        private void HandleFollowTarget()
        {
            if (_isDragging) return;

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
            if (minX > maxX) minX = maxX = bounds.center.x;
            if (minY > maxY) minY = maxY = bounds.center.y;

            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
            transform.position = clampedPos;
        }
        private bool IsPointerOverUI(int pointerId)
        {
            if (EventSystem.current == null) return false;
            if (pointerId == -1)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }
            return EventSystem.current.IsPointerOverGameObject(pointerId);
        }
        public void BlockInputForShortTime()
        {
            StopAllCoroutines();
            StartCoroutine(BlockInputRoutine());
        }

        private IEnumerator BlockInputRoutine()
        {
            _inputBlocked = true;
            _isDragging = false;
            _velocity = Vector3.zero;
            yield return new WaitForSeconds(0.2f);
            _inputBlocked = false;
        }

        public void SetTarget(Transform target)
        {
            _zoomTween?.Kill();
            _moveTween?.Kill();

            _target = target;

            Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);

            _moveTween = transform.DOMove(targetPos, 0.8f).SetEase(Ease.OutCirc);

            _zoomTween = DOTween.To(
                () => _mainCamera.orthographicSize,
                x => _mainCamera.orthographicSize = x,
                _minZoom,
                0.8f
            ).SetEase(Ease.OutCirc);
            
            AudioManager.Instance.PlaySFX("PlanetZoom");
        }

        public void ClearTarget()
        {
            _target = null;
            _zoomTween?.Kill();
            _zoomTween = DOTween.To(
                () => _mainCamera.orthographicSize,
                x => _mainCamera.orthographicSize = x,
                _unFocusMaxZoom,
                1f
            ).SetEase(Ease.OutCirc);
        }

        public void ResetDragState()
        {
            _isDragging = false;
            _velocity = Vector3.zero;
        }
        
        public void ResetCameraOnTarget(Transform targetToCenter)
        {
            _target = null;
            ResetDragState();
            _zoomTween?.Kill();
            _moveTween?.Kill();
            Vector3 targetPos = new Vector3(targetToCenter.position.x, targetToCenter.position.y, transform.position.z);
            _moveTween = transform.DOMove(targetPos, 1f).SetEase(Ease.OutCirc);
        }
    }
}