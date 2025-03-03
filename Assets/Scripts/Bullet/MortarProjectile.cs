using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarProjectile : Bullet
{
    [Header("Component")] 
    public GameObject model;
    public SphereCollider hurtArea;
    public Timer moveTimer;
    
    // [Header("Settings")]
    //[Header("Debug")]


    public override void Initialize(Vector3 pos, Quaternion rot, float offset = 0)
    {
        model.transform.position = pos;
        moveTimer.time = projectileDetails.hitTimerDelay;
        hurtArea.enabled = false;
        hurtArea.radius = projectileDetails.projectileHurtRadius;
    }

    protected override void InitialComponent()
    {
        hurtArea = GetComponentInChildren<SphereCollider>();
        base.InitialComponent();
    }

    private void FixedUpdate()
    {
        if (moveTimer.isPlay)
            model.transform.position = CalculateParabolaPoint(
                model.transform.position, 
                hurtArea.transform.position, 10,
                moveTimer.currentTime / moveTimer.time);
    }
    
    public static Vector3 CalculateParabolaPoint(Vector3 start, Vector3 end, float height, float t)
    {
        // 基本線性插值：在起點和終點之間計算位置
        Vector3 linearPoint = Vector3.Lerp(start, end, t);
        // 使用拋物線公式 4t(1-t) 獲得一個在 t=0.5 時達到最大值 1 的值，
        // 然後乘上 height 獲得垂直方向的偏移量
        float parabolicOffset = 4 * t * (1 - t);
        // 返回計算結果，將垂直偏移加到線性位置上
        return linearPoint + Vector3.up * height * parabolicOffset;
    }
}
