using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class TeamManager : Singleton<TeamManager>
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    public Team selfTeam;

    private void OnEnable()
    {
        // EventHandler.OnPlayerSpawned += OnPlayerSpawned;
    }
    
    private void OnDisable()
    {
        // EventHandler.OnPlayerSpawned -= OnPlayerSpawned;
    }

    private void OnPlayerSpawned()
    {
        selfTeam = NetworkManager.Singleton.IsHost ? Team.Blue : Team.Red;
    }
}
