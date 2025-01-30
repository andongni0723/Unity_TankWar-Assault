using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    public PoolKey poolKey;

    protected void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReleaseObject(poolKey, gameObject);
    }
}