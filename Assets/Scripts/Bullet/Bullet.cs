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
    public ProjectileDetailsSO projectileDetails;
    public float speed = 10;
    [Range(0, 10)] public int damage = 1;
    public PoolKey VFXPoolKey = PoolKey.RedHitVFX;
    
    //[Header("Debug")]
    private bool _isReleased;

    private void Awake()
    {
        InitialComponent();
    }
    
    private void OnEnable()
    {
        destroyTimer.OnTimerEnd += BackToPoolWithEffect;
        _isReleased = false;
    }

    private void OnDisable()
    {
        destroyTimer.OnTimerEnd -= BackToPoolWithEffect;
        _trailRenderer.Clear();
    }
    
    /// <summary>
    /// Call by CharacterShoot.cs when bullet is released
    /// </summary>
    /// <param name="pos">start position</param>
    /// <param name="rot">start direction</param>
    public void Initialize(Vector3 pos, Quaternion rot, float offset = 0)
    {
        transform.position = pos;
        transform.rotation = rot;
        transform.Rotate(Vector3.forward, offset);
        _rb.linearVelocity = transform.up * speed; // Reset velocity
    }
    
    private void InitialComponent()
    {
        _rb = GetComponent<Rigidbody>();
        _trailRenderer = GetComponent<TrailRenderer>();
        damage = projectileDetails.projectileDamage;
        destroyTimer.time = projectileDetails.projectileLifeTime;
    }
    
    private void BackToPoolWithEffect()
    {
        if (_isReleased) return;
        _isReleased = true;
        ExecuteOnEndEffect();
        ExecuteOnEndSkill();
        SpawnHitVFX();
        ReturnToPool();
    }
    
    private void SpawnHitVFX()
    {
        var hitVFX = ObjectPoolManager.Instance.GetObject(VFXPoolKey);
        hitVFX.transform.position = transform.position;
        hitVFX.transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAttack>(out var target))
        {
            target.TakeDamage(damage);
            BackToPoolWithEffect();
        }
    }
    
    private void ExecuteOnEndEffect()
    {
        
    }

    private void ExecuteOnEndSkill()
    {
        foreach (var skill in projectileDetails.onEndSkills)
        {
            var skillObj = ObjectPoolManager.Instance.GetObject(skill.skillPrefabPoolKey);
            skillObj.GetComponent<SkillBase>().Initialize(skill);
            skillObj.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
}
