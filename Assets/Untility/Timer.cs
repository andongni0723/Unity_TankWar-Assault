using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class Timer : MonoBehaviour
{
    //[Header("Component")]
    [Header("Settings")]
    [Range(0.1f, 100)]public float time = 1;
    public bool isLoop = false;
    public bool onAwakePlay = false;
    
    //[Header("Debug")]
    public UnityAction OnTimerEnd;
    public bool isPlay { get; private set; }
    public float currentTime { get; private set; }
    
    private void OnEnable()
    {
        currentTime = 0;
        if (onAwakePlay) Play();
    }

    private void Update()
    {
        if(isLoop || isPlay) TimeUpdate();
    }

    private void TimeUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentTime < time) return;
        
        // When Timer End
        isPlay = false;
        currentTime = 0;
        OnTimerEnd?.Invoke();
        if(isLoop) isPlay = true;
    }

    #region Tools

    /// <summary>
    /// Call this method to start the timer.
    /// </summary>
    public void Play()
    {
        isPlay = true;
    }
    
    /// <summary>
    /// Call this method to stop the timer.
    /// </summary>
    public void Stop()
    {
        isPlay = false;
    } 
    #endregion
}
