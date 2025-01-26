using System;

public static class EventHandler
{
    public static Action<WeaponDetailsSO> OnWeaponSelectToggleSelected;
    public static void CallOnWeaponSelectToggleSelected(WeaponDetailsSO weaponDetails)
    {
        OnWeaponSelectToggleSelected?.Invoke(weaponDetails);
    }
    
    
    public static Action<CharacterController> OnPlayerSpawned;
    public static void CallOnPlayerSpawned(CharacterController characterController)
    {
        OnPlayerSpawned?.Invoke(characterController);
    }
    
    public static Action OnAllPlayerSpawned;
    public static void CallOnAllPlayerSpawned()
    {
        OnAllPlayerSpawned?.Invoke();
    }
    
    
    public static Action<CharacterController> OnOwnerSpawned;
    public static void CallOnOwnerSpawned(CharacterController characterController)
    {
        OnOwnerSpawned?.Invoke(characterController);
    }
    
    public static Action<bool> OnPlayerDied;
    public static void CallOnPlayerDied(bool isOwner)
    {
        OnPlayerDied?.Invoke(isOwner);
    }
}