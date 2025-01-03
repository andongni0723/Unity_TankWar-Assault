using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Pool;

[SerializeField]
public class PoolObject
{
    public string key;
    public IObjectPool<GameObject> prefab;
    public int defaultCapacity;
    public int maxSize;
}

public enum PoolKey
{
    RedBullet, BlueBullet, 
    RedHitVFX, BlueHitVFX
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [Header("Debug")]
    private static ObjectPoolManager _instance;
    
    private Dictionary<PoolKey, IObjectPool<GameObject>> _pools = new();

    [Header("Settings")]
    public List<PoolDetailsSO> poolDetailes_SO = new();

    public override void Awake()
    {
        base.Awake();
        UseDetailsRegisterPool();
    }

    private void UseDetailsRegisterPool()
    {
        foreach (var data in poolDetailes_SO)
            RegisterPool(data.poolKey, data.prefab, data.defaultCapacity, data.maxSize); 
    }

    // 註冊一個物件池
    public void RegisterPool(PoolKey key, GameObject prefab, int defaultCapacity = 10, int maxSize = 50)
    {
        if (_pools.ContainsKey(key))
        {
            Debug.LogWarning($"Pool with key {key} already exists.");
            return;
        }

        var pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab, parent: transform),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools.Add(key, pool);
    }

    // 取得物件
    public GameObject GetObject(PoolKey key)
    {
        if (!_pools.TryGetValue(key, out var pool))
        {
            Debug.LogError($"Pool with key {key} does not exist.");
            return null;
        }
        return pool.Get();
    }

    // 回收物件
    public void ReleaseObject(PoolKey key, GameObject obj)
    {
        if (!_pools.TryGetValue(key, out var pool))
        {
            Debug.LogError($"Pool with key {key} does not exist.");
            return;
        }
        pool.Release(obj);
    }
}