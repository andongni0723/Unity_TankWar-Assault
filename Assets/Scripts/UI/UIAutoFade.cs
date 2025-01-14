using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Sequence = DG.Tweening.Sequence;

[RequireComponent(typeof(CanvasGroup))]
public class UIAutoFade : MonoBehaviour
{
    [Header("Component")] 
    private CanvasGroup _canvasGroup;
    
    [Header("Settings")] 
    public Timer waitFadeTimer;
    
    // [Header("Debug")]
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        waitFadeTimer.OnTimerEnd += FadeOut;
    }

    /// <summary>
    /// Interrupt the fade out process and reset the timer. Add this method on the UI element action.
    /// </summary>
    public void ResetWaitFadeTimer()
    {
        waitFadeTimer.Play();
        _canvasGroup.DOKill();
        _canvasGroup.alpha = 1;
    }

    private void FadeOut()
    {
        _canvasGroup.DOFade(0.2f, 1);
    }
}