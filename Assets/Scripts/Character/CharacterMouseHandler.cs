using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CharacterMouseHandler : MonoBehaviour
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    
    [Header("Drag Areas")]
    public List<RectTransform> dragAreas; // 可拖曳區域的父子組合

    public bool isDragging; // 是否正在拖曳
    public Vector2 mouseDelta;

    private void Start()
    {
        if (!GameDataManager.Instance.canDragCamera) enabled = false;
        dragAreas = GameUIManager.Instance.dragAreas;
    }

    private void Update()
    {
        if (!isDragging) return;
        
#if UNITY_EDITOR    
        mouseDelta = Mouse.current.delta.ReadValue();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        mouseDelta = Mouse.current.delta.ReadValue();
#elif UNITY_ANDROID || UNITY_IOS
        mouseDelta = Touchscreen.current.delta.ReadValue();
#endif
    }

    public void OnMouseClickStarted()
    {
        if (IsMouseInAnyDragArea())
            isDragging = true;
    }

    public void OnMouseClickCanceled() => isDragging = false;

    private bool IsMouseInAnyDragArea()
    {
#if UNITY_EDITOR 
        return dragAreas.Any(area => RectTransformUtility.RectangleContainsScreenPoint(area, Mouse.current.position.ReadValue()));
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        return dragAreas.Any(area => RectTransformUtility.RectangleContainsScreenPoint(area, Mouse.current.position.ReadValue()));
#elif UNITY_ANDROID || UNITY_IOS
        return dragAreas.Any(area => RectTransformUtility.RectangleContainsScreenPoint(area, Touchscreen.current.position.ReadValue()));
#endif
    }
}
