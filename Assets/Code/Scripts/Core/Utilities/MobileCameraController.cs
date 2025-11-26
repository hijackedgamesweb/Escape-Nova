using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Code.Scripts.Core.Utilities
{
    public class MobileCameraController : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 0.5f;
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 15f;
        
        [Header("Movement Settings")]
        [SerializeField] private float panSpeed = 0.5f;
        [SerializeField] private Vector2 mapLimitsMin = new Vector2(-50, -50);
        [SerializeField] private Vector2 mapLimitsMax = new Vector2(50, 50);

        private UnityEngine.Camera _cam;
        private Vector3 _dragOrigin;
        private bool _isDragging = false;
        private bool _inputBlocked = false;

        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            EnhancedTouchSupport.Enable();
        }

        private void OnDestroy()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            if (Time.timeScale == 0 || _inputBlocked) return;

            var touches = Touch.activeTouches;

            if (touches.Count == 0)
            {
                _isDragging = false;
                return;
            }
            if (touches.Count == 2)
            {
                _isDragging = false;
                HandleZoom(touches[0], touches[1]);
            }
            else if (touches.Count == 1)
            {
                Touch touch = touches[0];
                if (IsPointerOverUI(touch)) 
                {
                    _isDragging = false;
                    return;
                }

                HandlePan(touch);
            }
        }
        private bool IsPointerOverUI(Touch touch)
        {
            int id = touch.touchId;
            return EventSystem.current.IsPointerOverGameObject(id);
        }

        private void HandlePan(Touch touch)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                _dragOrigin = _cam.ScreenToWorldPoint(touch.screenPosition);
                _isDragging = true;
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && _isDragging)
            {
                Vector3 currentPos = _cam.ScreenToWorldPoint(touch.screenPosition);
                Vector3 difference = _dragOrigin - currentPos;
                
                Vector3 newPos = transform.position + difference;
                newPos.x = Mathf.Clamp(newPos.x, mapLimitsMin.x, mapLimitsMax.x);
                newPos.y = Mathf.Clamp(newPos.y, mapLimitsMin.y, mapLimitsMax.y);
                
                transform.position = newPos;
            }
        }

        private void HandleZoom(Touch touch0, Touch touch1)
        {
            Vector2 touch0PrevPos = touch0.screenPosition - touch0.delta;
            Vector2 touch1PrevPos = touch1.screenPosition - touch1.delta;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.screenPosition - touch1.screenPosition).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            float newSize = _cam.orthographicSize + (deltaMagnitudeDiff * zoomSpeed * 0.01f);
            _cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
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
            yield return new WaitForSeconds(0.2f);
            _inputBlocked = false;
        }
    }
}