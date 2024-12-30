using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [Header("Component")]
    
    [Header("Settings")]
    public float duration = 0.1f;
    
    [Header("Panel Animation")]
    public bool PlayOnAwake = false;
    public float waitTime = 0.1f;
    public float fromPositionX = -1000;
    
    [Header("Button Animation")]
    public float downScale = 0.8f;
    public float upScale = 1.1f; // 增加一點回彈效果
    public float normalScale = 1f; 
    
    //[Header("Debug")]
    
    private void Awake()
    {
        if (PlayOnAwake)
            PanelLeftInAnimation(waitTime);
        
        if(PlayOnAwake)
            InvokeRepeating(nameof(PanelLeftInAnimation), 2f, 2f);
        
    }
    
    public void PanelLeftInAnimation(float _waitTime = 0)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(_waitTime);
        sequence.Append(transform.DOLocalMoveX(fromPositionX, duration).SetEase(Ease.OutCubic).From());
    }
    
    public void ButtonPointerDownAnimation()
    {
        transform.DOScale(downScale, duration * 0.5f).SetEase(Ease.OutElastic);
    }
    
    public void ButtonPointerUpAnimation()
    {
        transform.DOScale(normalScale, duration * 0.5f).SetEase(Ease.InElastic);
    }
    
}
