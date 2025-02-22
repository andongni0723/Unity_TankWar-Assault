using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Component")]
    public GameObject blueTankSpawnPoint;
    public GameObject redTankSpawnPoint;
    public CinemachineCamera skyCamera;
    
    [Space(15f)]
    public GameObject advantageBlueTankSpawnPoint;
    public GameObject advantageRedTankSpawnPoint;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void OnEnable()
    {
        EventHandler.OnPlayerDied += OnPlayerDied;
        EventHandler.OnPlayerRespawn += OnPlayerRespawn;
    }

    private void OnDisable()
    {
        EventHandler.OnPlayerDied -= OnPlayerDied;
        EventHandler.OnPlayerRespawn -= OnPlayerRespawn;
    }

    private void OnPlayerDied(bool isOwner, Timer respawnTimer)
    {
        if (isOwner)
            skyCamera.gameObject.SetActive(true);
    }

    private void OnPlayerRespawn(bool isOwner)
    {
        if (isOwner)
            skyCamera.gameObject.SetActive(false);
    }

    public GameObject GetStartSpawnPoint(Team team)
    {
        return team == Team.Blue ? blueTankSpawnPoint : redTankSpawnPoint;
    }
}
