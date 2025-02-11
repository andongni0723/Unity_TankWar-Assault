using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;


public class AreaManager : MonoBehaviour
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    public List<AreaData> areaDataList = new();

    private void OnEnable()
    {
        EventHandler.OnAllPlayerSpawned += OnAllPlayerSpawned;
    }

    private void OnAllPlayerSpawned()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // areaDataList[0].CallUpdateOccupiedPercentage(Team.Blue, 100); // SB
            areaDataList[1].CallUpdateOccupiedPercentage(Team.Blue, 50); // AB
            areaDataList[3].CallUpdateOccupiedPercentage(Team.Red, 50); // AR
            // areaDataList[4].CallUpdateOccupiedPercentage(Team.Red, 100); // SR 
        }
    }
}
