using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class MortarProjectile : Bullet
{
    [Header("Component")] 
    public GameObject model;
    public GameObject warningArea;
    public SphereCollider hurtArea;
    // public Timer moveTimer;
    
    [Header("Settings")]
    public float moveHeight = 10;
    
    [Header("Debug")] private Vector3 _targetPoint;
    
    private const float _HURT_RADIUS_TO_DECAL_RADIUS = 5; // 10 / 2
    private const float _HURT_RADIUS_TO_WARNING_SCALE = 3.8f; // 7.6 / 2

    protected override void InitialComponent()
    {
        hurtArea = GetComponent<SphereCollider>();
        base.InitialComponent();
    }

    public override void Initialize(Vector3 startPos, Vector3 targetPos, float offset = 0)
    {
        hurtArea.enabled = false;
        _targetPoint = new Vector3(targetPos.x + offset, 1, targetPos.z + offset);
        transform.position = startPos;
        model.transform.localPosition = Vector3.zero;
        destroyTimer.time = projectileDetails.hitTimerDelay;
        hurtArea.radius = projectileDetails.projectileHurtRadius * _HURT_RADIUS_TO_DECAL_RADIUS;
        warningArea.transform.localScale = Vector3.one * (projectileDetails.projectileHurtRadius * _HURT_RADIUS_TO_WARNING_SCALE);
        warningArea.SetActive(true);
        model.SetActive(true);
        MoveAction();
    }

    private void Update()
    {
        UpdateWarningArea();
    }

    private void UpdateWarningArea()
    {
        warningArea.transform.position = _targetPoint;
    }

    private void MoveAction()
    {
        Sequence sequence = DOTween.Sequence();
        // this object move x pos and z pos to target point
        sequence.Append(transform.DOMove(_targetPoint, projectileDetails.hitTimerDelay).SetEase(Ease.Linear));
        // model object move y pos to up and down
        sequence.Join(model.transform
            .DOLocalMoveY(moveHeight, projectileDetails.hitTimerDelay / 2)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                model.transform.DOLocalMoveY(0, projectileDetails.hitTimerDelay / 2).SetEase(Ease.InQuad);
            })
        );
        // execute the explosion effect
        sequence.OnComplete(() =>
        {
            hurtArea.enabled = true;
            model.SetActive(false);
        });
    }
    
    protected override void BackToPoolWithEffect()
    {
        if (_isReleased) return;
        _isReleased = true;
        EventHandler.CallCameraShake(10, 1, 0.5f);
        ExecuteOnEndEffect();
        ExecuteOnEndSkill();
        SpawnHitVFX();
        Invoke(nameof(ReturnToPool), 1f);
    }
}
