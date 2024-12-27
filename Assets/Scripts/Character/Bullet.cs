using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableObject
{
    [Header("Component")]
    private Rigidbody _rb;
    
    [Header("Settings")]
    public float speed = 10;
    public PoolKey VFXPoolKey = PoolKey.RedHitVFX;
    //[Header("Debug")]

    private void Awake()
    {
        InitialComponent();
        StartCoroutine(BackToPool());
    }
    
    IEnumerator BackToPool()
    {
        yield return new WaitForSeconds(1);
        // Instantiate(HitVFX, transform.position, Quaternion.identity);
        var hitVFX = ObjectPoolManager.Instance.GetObject(VFXPoolKey);
        hitVFX.transform.position = transform.position;
        hitVFX.transform.rotation = Quaternion.identity;
        ReturnToPool();
    }

    private void InitialComponent()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.up * speed;
    }
}
