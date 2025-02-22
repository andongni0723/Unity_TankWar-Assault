using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WorldSpaceAreaUI : AreaUI
{
    [Header("Component")]
    private CanvasGroup _canvasGroup;
    
    //[Header("Settings")]
    //[Header("Debug")]
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        EventHandler.OnPlayerDied += AreaUIActive;
        EventHandler.OnPlayerRespawn += AreaUIClose;
    }

    private void OnDisable()
    {
        EventHandler.OnPlayerDied -= AreaUIActive;
        EventHandler.OnPlayerRespawn -= AreaUIClose;
    }
    
    private void AreaUIActive(bool isOwner, Timer arg2)
    {
        if(!isOwner) return;
        _canvasGroup.DOFade(1f, 0.3f);
    }
    
    private void AreaUIClose(bool isOwner)
    {
        if(!isOwner) return;
        _canvasGroup.DOFade(0, 0.3f);
    }

   
}
