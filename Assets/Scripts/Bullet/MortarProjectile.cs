using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class MortarProjectile : Bullet
{
    [Header("Component")] 
    public GameObject model;
    public SphereCollider hurtArea;
    // public Timer moveTimer;
    
    [Header("Settings")]
    public float moveHeight = 10;
    
    [Header("Debug")] private Vector3 _targetPoint;

    protected override void InitialComponent()
    {
        // hurtArea = GetComponentInChildren<SphereCollider>();
        hurtArea = GetComponent<SphereCollider>();
        base.InitialComponent();
    }

    public override void Initialize(Vector3 startPos, Vector3 targetPos, float offset = 0)
    {
        hurtArea.enabled = false;
        _targetPoint = new Vector3(targetPos.x + offset, 1, targetPos.z + offset);
        transform.position = startPos;
        model.transform.localPosition = Vector3.zero;
        // moveTimer.time = projectileDetails.hitTimerDelay;
        destroyTimer.time = projectileDetails.hitTimerDelay;
        hurtArea.radius = projectileDetails.projectileHurtRadius;
        MoveAction();
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
            // Invoke(nameof(BackToPoolWithEffect), 1);
        });
    }

    // protected override void OnTriggerEnter(Collider other)
    // {
    //     // Projectile will back to pool 不管 it whether hit the enemy
    //     base.OnTriggerEnter(other);
    //     BackToPoolWithEffect();
    // }

    // public static Vector3 CalculateParabolaPoint(Vector3 start, Vector3 end, float height, float t)
    // {
    //     // 基本線性插值：在起點和終點之間計算位置
    //     Vector3 linearPoint = Vector3.Lerp(start, end, t);
    //     // 使用拋物線公式 4t(1-t) 獲得一個在 t=0.5 時達到最大值 1 的值，
    //     // 然後乘上 height 獲得垂直方向的偏移量
    //     float parabolicOffset = 4 * t * (1 - t);
    //     // 返回計算結果，將垂直偏移加到線性位置上
    //     return linearPoint + Vector3.up * height * parabolicOffset;
    // }
}
