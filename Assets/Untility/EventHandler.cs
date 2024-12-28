using UnityEngine.Events;

public class EventHandler
{
    public static UnityAction OnPlayerSpawned;
    public static void CallOnPlayerSpawned()
    {
        OnPlayerSpawned?.Invoke();
    }
}