using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Timer : MonoBehaviour
{
    //[Header("Component")]
    [Header("Settings")]
    [Range(0.1f, 100)]public float time = 1;
    public bool isLoop = false;
    public bool onAwakePlay = false;
    
    //[Header("Debug")]
    public UnityAction OnTimerEnd;
    private bool _isPlay;
    private float _timer;
    
    private void OnEnable()
    {
        _timer = 0;
        if (onAwakePlay) Play();
    }

    private void Update()
    {
        if(isLoop || _isPlay) TimeUpdate();
    }

    private void TimeUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer < time) return;
        
        // When Timer End
        _isPlay = false;
        _timer = 0;
        OnTimerEnd?.Invoke();
        if(isLoop) _isPlay = true;
    }

    #region Tools

    /// <summary>
    /// Call this method to start the timer.
    /// </summary>
    public void Play()
    {
        _isPlay = true;
    }
    
    /// <summary>
    /// Call this method to stop the timer.
    /// </summary>
    public void Stop()
    {
        _isPlay = false;
    } 
    #endregion
}
