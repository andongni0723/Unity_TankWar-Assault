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
    
    private CharacterController blueTeamCharacter;
    private CharacterController redTeamCharacter;

    private void OnEnable()
    {
        EventHandler.OnPlayerSpawned += OnPlayerSpawned;
    }
    
    private void OnDisable()
    {
        EventHandler.OnPlayerSpawned -= OnPlayerSpawned;
    }

    private void OnPlayerSpawned(CharacterController character)
    {
        (character.team.Value == Team.Blue ? ref blueTeamCharacter : ref redTeamCharacter) = character;
    }
    
    public CharacterController GetEnemyTeamCharacterController()
    {
        return NetworkManager.Singleton.IsHost ? redTeamCharacter : blueTeamCharacter;
    }

}
