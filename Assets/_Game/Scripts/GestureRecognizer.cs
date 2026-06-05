using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Pully.Game
{
    /// <summary>
    /// Handles touch input and recognizes the 5 gestures defined in the ruleset:
    /// - Single tap: one quick tap
    /// - Double tap: two taps < 300ms
    /// - Long press: hold > 500ms
    /// - Swipe-tap: tap + directional flick
    /// - Two-finger tap: two simultaneous touches
    /// </summary>
    public class GestureRecognizer : MonoBehaviour
    {
        [SerializeField] private RulesetDefinition ruleset;
        
        public event Action<Vector2> OnSingleTap;
        public event Action<Vector2> OnDoubleTap;
        public event Action<Vector2> OnLongPress;
        public event Action<Vector2, Vector2> OnSwipeTap;
        public event Action<Vector2> OnTwoFingerTap;
        public event Action<Vector2> OnTouchBegan;
        
        private Dictionary<int, TouchData> activeTouches = new();
        private Dictionary<int, DateTime> lastTapTimes = new();
        
        private class TouchData
        {
            public Vector2 StartPosition;
            public DateTime StartTime;
            public bool IsHolding;
            public bool HasTriggeredLongPress;
        }
        
        private Camera mainCamera;
        private float doubleTapWindow;
        private float longPressDuration;
        private float swipeMinDistance;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            if (ruleset == null)
            {
                Debug.LogError("[GestureRecognizer] Ruleset not assigned!");
                return;
            }
            doubleTapWindow = ruleset.doubleTapWindow;
            longPressDuration = ruleset.longPressDuration;
            swipeMinDistance = ruleset.swipeMinDistance;
        }
        
        private void OnEnable() => EnhancedTouchSupport.Enable();
        private void OnDisable() => EnhancedTouchSupport.Disable();
        
        private void Update()
        {
            foreach (var touch in Touch.activeTouches)
            {
                int touchId = touch.touchId;
                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        HandleTouchBegan(touchId, touch.screenPosition);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Ended:
                        HandleTouchEnded(touchId, touch.screenPosition);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                        HandleTouchCanceled(touchId);
                        break;
                }
            }
            CheckLongPresses();
            HandleMouseInput();
        }
        
        private void HandleMouseInput()
        {
            if (Mouse.current == null) return;
            if (Mouse.current.leftButton.wasPressedThisFrame)
                HandleTouchBegan(-1, Mouse.current.position.value);
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
                HandleTouchEnded(-1, Mouse.current.position.value);
            if (Mouse.current.rightButton.wasPressedThisFrame)
                OnTwoFingerTap?.Invoke(Mouse.current.position.value);
        }
        
        private void HandleTouchBegan(int touchId, Vector2 screenPos)
        {
            bool isDoubleTap = lastTapTimes.TryGetValue(touchId, out DateTime lastTap) && 
                               (DateTime.Now - lastTap).TotalSeconds <= doubleTapWindow;
            
            activeTouches[touchId] = new TouchData
            {
                StartPosition = screenPos,
                StartTime = DateTime.Now,
                IsHolding = true,
                HasTriggeredLongPress = false
            };
            
            OnTouchBegan?.Invoke(screenPos);
            if (isDoubleTap)
            {
                OnDoubleTap?.Invoke(screenPos);
                activeTouches.Remove(touchId);
            }
        }
        
        private void HandleTouchEnded(int touchId, Vector2 screenPos)
        {
            if (!activeTouches.TryGetValue(touchId, out TouchData touchData)) return;
            if (touchData.HasTriggeredLongPress)
            {
                activeTouches.Remove(touchId);
                return;
            }
            
            Vector2 direction = screenPos - touchData.StartPosition;
            float distance = direction.magnitude;
            float duration = (float)(DateTime.Now - touchData.StartTime).TotalSeconds;
            
            if (distance >= swipeMinDistance)
                OnSwipeTap?.Invoke(touchData.StartPosition, direction.normalized);
            else if (duration < longPressDuration)
            {
                OnSingleTap?.Invoke(screenPos);
                lastTapTimes[touchId] = DateTime.Now;
            }
            activeTouches.Remove(touchId);
        }
        
        private void HandleTouchCanceled(int touchId) => activeTouches.Remove(touchId);
        
        private void CheckLongPresses()
        {
            DateTime now = DateTime.Now;
            foreach (var kvp in activeTouches)
            {
                var touchData = kvp.Value;
                if (touchData.IsHolding && !touchData.HasTriggeredLongPress)
                {
                    float duration = (float)(now - touchData.StartTime).TotalSeconds;
                    if (duration >= longPressDuration)
                    {
                        touchData.HasTriggeredLongPress = true;
                        OnLongPress?.Invoke(touchData.StartPosition);
                    }
                }
            }
        }
        
        public RulesetDefinition.Gesture GetRequiredGesture(RulesetDefinition.Shape shape, Color color)
        {
            foreach (var rule in ruleset.rules)
                if (rule.shape == shape && ColorsMatch(rule.color, color))
                    return rule.requiredGesture;
            return RulesetDefinition.Gesture.SingleTap;
        }
        
        private bool ColorsMatch(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.01f && Mathf.Abs(a.g - b.g) < 0.01f && Mathf.Abs(a.b - b.b) < 0.01f;
        }
    }
}
