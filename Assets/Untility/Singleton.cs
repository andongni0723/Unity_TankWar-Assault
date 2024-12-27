using UnityEngine;
using Sirenix.OdinInspector;

public class Singleton<T> : SerializedMonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance => instance;

    public virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = (T)this;
        }
    }
}