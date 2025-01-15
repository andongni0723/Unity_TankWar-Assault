using UnityEngine.Events;

public static class EventHandler
{
    public static UnityAction<WeaponDetailsSO> OnWeaponSelectToggleSelected;
    public static void CallOnWeaponSelectToggleSelected(WeaponDetailsSO weaponDetails)
    {
        OnWeaponSelectToggleSelected?.Invoke(weaponDetails);
    }
    
    
    public static UnityAction<CharacterController> OnPlayerSpawned;
    public static void CallOnPlayerSpawned(CharacterController characterController)
    {
        OnPlayerSpawned?.Invoke(characterController);
    }
    
    
    public static UnityAction<CharacterController> OnOwnerSpawned;
    public static void CallOnOwnerSpawned(CharacterController characterController)
    {
        OnOwnerSpawned?.Invoke(characterController);
    }
    
    public static UnityAction<bool> OnPlayerDied;
    public static void CallOnPlayerDied(bool isOwner)
    {
        OnPlayerDied?.Invoke(isOwner);
    }
}