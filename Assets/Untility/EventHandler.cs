using UnityEngine.Events;

public static class EventHandler
{
    public static UnityAction OnPlayerSpawned;
    public static void CallOnPlayerSpawned()
    {
        OnPlayerSpawned?.Invoke();
    }
    
    public static UnityAction<bool> OnPlayerDied;
    public static void CallOnPlayerDied(bool isOwner)
    {
        OnPlayerDied?.Invoke(isOwner);
    }
}