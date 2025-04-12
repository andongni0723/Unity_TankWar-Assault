using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CharacterMouseHandler : MonoBehaviour
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    
    [Header("Drag Areas")]
    public List<RectTransform> dragAreas;
    public bool isDragging;
    public Vector2 mouseDelta;
    
    #if UNITY_ANDROID || UNITY_IOS
    private TouchControl _mobileTouch;
    #endif
    
    private void Start() => Initialize();

    private void Update() => UpdateMouseDeltaValue();

    private void Initialize()
    {
        if (!GameDataManager.Instance.canDragCamera) enabled = false;
        dragAreas = GameUIManager.Instance.dragAreas;
    }

    private void UpdateMouseDeltaValue()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            mouseDelta = isDragging ? Mouse.current.delta.ReadValue() : Vector2.zero;
        #elif UNITY_ANDROID || UNITY_IOS
            mouseDelta = _mobileTouch != null && _mobileTouch.press.isPressed ? _mobileTouch.delta.ReadValue() : Vector2.zero;
        #endif
    }

    /// <summary>
    /// Call by CharacterController when "camera drag" input event is start
    /// </summary>
    public void OnMouseClickStarted()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (IsMouseInAnyDragArea()) isDragging = true;
        
        #elif UNITY_ANDROID || UNITY_IOS
            var touches = Touchscreen.current?.touches;
            if (touches == null) return;
            _mobileTouch = touches.Cast<TouchControl>().FirstOrDefault(IsTouchInAnyDragArea);
            isDragging = _mobileTouch != null; // Have finger in area or not
        #endif
    }

    /// <summary>
    /// Call by CharacterController when "camera drag" input event was canceled
    /// </summary>
    public void OnMouseClickCanceled() => isDragging = false;

    #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    private bool IsMouseInAnyDragArea() => 
        dragAreas.Any(area => 
            RectTransformUtility.RectangleContainsScreenPoint(area, Mouse.current.position.ReadValue()));
    #endif
    
    #if UNITY_ANDROID || UNITY_IOS
    private bool IsTouchInAnyDragArea(TouchControl touch) => 
        dragAreas.Any(area => 
            RectTransformUtility.RectangleContainsScreenPoint(area, touch.position.ReadValue()));
    #endif
}
