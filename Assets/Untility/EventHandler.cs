using System;
using UnityEngine;

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
        _callTime = 0;
    }
    
    public static Action OnAllPlayerSpawned;
    public static void CallOnAllPlayerSpawned()
    {
        OnAllPlayerSpawned?.Invoke();
    }

    public static Action OnGameStart;
    static int _callTime = 0;
    public static void CallOnGameStart()
    {
        if(_callTime > 0) return;
        Debug.Log("Start");
        OnGameStart?.Invoke();
        _callTime++;
    }
    
    public static Action<CharacterController> OnOwnerSpawned;
    public static void CallOnOwnerSpawned(CharacterController characterController)
    {
        OnOwnerSpawned?.Invoke(characterController);
    }
    
    public static Action<bool, Timer> OnPlayerDied;
    public static void CallOnPlayerDied(bool isOwner, Timer respawnTimer)
    {
        OnPlayerDied?.Invoke(isOwner, respawnTimer);
    }
    
    public static Action<bool> OnPlayerRespawn;
    public static void CallOnPlayerRespawn(bool isOwner)
    {
        OnPlayerRespawn?.Invoke(isOwner);
    } 
    
    public static Action<Team, AreaName> OnAreaOccupied;
    public static void CallOnAreaOccupied(Team team, AreaName areaName)
    {
        OnAreaOccupied?.Invoke(team, areaName);
    }
    
    public static Action<bool> OnGameEnd;
    public static void CallOnGameEnd(bool isHost)
    {
        OnGameEnd?.Invoke(isHost);
    }

    public static Action OnLeaveSession;
    public static void CallOnLeaveSession()
    {
        OnLeaveSession?.Invoke();
    }
}