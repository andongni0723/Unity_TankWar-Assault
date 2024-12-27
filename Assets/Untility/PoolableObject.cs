using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    public PoolKey poolKey;

    public void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReleaseObject(poolKey, gameObject);
    }
}