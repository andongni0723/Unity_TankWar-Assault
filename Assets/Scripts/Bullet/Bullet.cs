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
    private float _speed = 10;
    private int _damage = 1;
    public PoolKey VFXPoolKey = PoolKey.RedHitVFX;
    
    //[Header("Debug")]
    protected bool _isReleased;

    private void Awake()
    {
        InitialComponent();
    }
    
    protected void OnEnable()
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
    /// <param name="offset">bullet shoot direction offset</param>
    public virtual void Initialize(Vector3 pos, Quaternion rot, float offset = 0)
    {
        transform.position = pos;
        transform.rotation = rot;
        transform.Rotate(Vector3.forward, offset);
        _rb.linearVelocity = transform.up * _speed; // Reset velocity
    }

    /// <summary>
    /// Call by CharacterShoot.cs when bullet is released
    /// </summary>
    /// <param name="startPos">start position</param>
    /// <param name="targetPos">target position</param>
    /// <param name="offset">bullet target position offset</param>
    public virtual void Initialize(Vector3 startPos, Vector3 targetPos, float offset = 0)
    {
        var p = transform.position;
        _rb.linearVelocity = (targetPos - startPos).normalized * _speed;
        p = startPos;
        p += new Vector3(offset, 1, offset);
    }
    
    
    protected virtual void InitialComponent()
    {
        _rb = GetComponent<Rigidbody>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _damage = projectileDetails.projectileDamage;
        _speed = projectileDetails.projectileSpeed;
        destroyTimer.time = projectileDetails.projectileLifeTime;
    }
    
    protected virtual void BackToPoolWithEffect()
    {
        if (_isReleased) return;
        _isReleased = true;
        ExecuteOnEndEffect();
        ExecuteOnEndSkill();
        SpawnHitVFX();
        ReturnToPool();
    }

    protected void SpawnHitVFX()
    {
        var hitVFX = ObjectPoolManager.Instance.GetObject(VFXPoolKey);
        hitVFX.transform.position = transform.position;
        hitVFX.transform.rotation = Quaternion.identity;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAttack>(out var target))
        {
            target.TakeDamage(_damage);
            BackToPoolWithEffect();
        }
    }

    protected void ExecuteOnEndEffect()
    {
        
    }

    protected void ExecuteOnEndSkill()
    {
        foreach (var skill in projectileDetails.onEndSkills)
        {
            var skillObj = ObjectPoolManager.Instance.GetObject(skill.skillPrefabPoolKey);
            skillObj.GetComponent<SkillBase>().Initialize(skill);
            skillObj.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
}
