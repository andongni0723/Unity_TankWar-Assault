using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;


public enum UIType
{
    Panel,
    Button,
}

public class UIAnimation : MonoBehaviour
{
    [Header("Component")]
    
    [Header("Settings")]
    public float duration = 0.1f;

    [Space(15)]
    public UIType type;
    [ShowIf("type", UIType.Panel)] public bool PlayOnAwake = false;
    [ShowIf("type", UIType.Panel)] [ShowIf("PlayOnAwake", true)] public float waitTime = 0.1f;
    [ShowIf("type", UIType.Panel)] [ShowIf("PlayOnAwake", true)] public float fromPositionX = -1000;
    [ShowIf("type", UIType.Panel)] [ShowIf("PlayOnAwake", true)] public float targetPositionX = -1000;
 
    
    [ShowIf("type", UIType.Button)] public float downScale = 0.8f;
    [ShowIf("type", UIType.Button)] public float upScale = 1.1f; // 增加一點回彈效果
    [ShowIf("type", UIType.Button)] public float normalScale = 1f; 
    
    //[Header("Debug")]
    [HideInInspector] public Vector3 startPos;

    private void Awake()
    {
        startPos = transform.localPosition;
    }

    private void Start()
    {
        if (PlayOnAwake)
            PanelLeftInAnimation(startPos.x, waitTime);
    }

    public void PanelLeftInAnimation(float _targetPositionX = 1200, float _waitTime = 0)
    {
        gameObject.SetActive(false);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveX(2500, 0)
                .OnComplete(() => gameObject.SetActive(true)));
        sequence.AppendInterval(_waitTime);
        sequence.Append(transform.DOLocalMoveX(_targetPositionX, duration).SetEase(Ease.OutCubic));
    }
    
    public void PanelRightOutAnimation(float _waitTime = 0)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(_waitTime);
        sequence.Append(transform.DOLocalMoveX(2500, duration).SetEase(Ease.OutCubic));
        sequence.OnComplete(() => gameObject.SetActive(false));
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
