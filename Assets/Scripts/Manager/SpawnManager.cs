using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Component")]
    public GameObject blueTankSpawnPoint;
    public GameObject redTankSpawnPoint;
    
    [Space(15f)]
    public GameObject advantageBlueTankSpawnPoint;
    public GameObject advantageRedTankSpawnPoint;
    
    //[Header("Settings")]
    //[Header("Debug")]

    public GameObject GetStartSpawnPoint(Team team)
    {
        return team == Team.Blue ? blueTankSpawnPoint : redTankSpawnPoint;
    }
}
