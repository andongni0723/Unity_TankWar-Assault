using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableObject
{
    [Header("Component")]
    public Timer destroyTimer;
    private Rigidbody _rb;
    private TrailRenderer _trailRenderer;
    
    [Header("Settings")]
    public float speed = 10;
    [Range(0, 10)] public int damage = 1;
    public PoolKey VFXPoolKey = PoolKey.RedHitVFX;
    
    //[Header("Debug")]
    private bool isReleased;

    private void Awake()
    {
        InitialComponent();
    }

    private void OnEnable()
    {
        isReleased = false;
        destroyTimer.OnTimerEnd += BackToPool;
    }

    private void OnDisable()
    {
        destroyTimer.OnTimerEnd -= BackToPool;
        _trailRenderer.Clear();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.up * speed;
    }
    
    private void InitialComponent()
    {
        _rb = GetComponent<Rigidbody>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }
    
    private void BackToPool()
    {
        if (isReleased) return;
        isReleased = true;
        var hitVFX = ObjectPoolManager.Instance.GetObject(VFXPoolKey);
        hitVFX.transform.position = transform.position;
        hitVFX.transform.rotation = Quaternion.identity;
        ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAttack>(out var target))
        {
            target.TakeDamage(damage);
            BackToPool();
        }
    }
}
