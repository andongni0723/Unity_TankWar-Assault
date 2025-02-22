using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class AreaDetails
{
    public AreaName areaName;
    public AreaController areaController;
    public List<AreaUI> AreaUIList = new();
}

public class AreaManager : Singleton<AreaManager>
{
    [Header("Component")]
    public GameObject areaDataPrefab;
    
    [Header("Settings")]
    public List<AreaDetails> areaDetailsList = new();
    private List<AreaData> _areaDataList = new();
    //[Header("Debug")]

    private void OnEnable()
    {
        EventHandler.OnAllPlayerSpawned += OnAllPlayerSpawned;
    }
    
    private void OnDisable()
    {
        EventHandler.OnAllPlayerSpawned -= OnAllPlayerSpawned;
    }

    #region Initialization
    private void OnAllPlayerSpawned()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        GenerateAreaDataObject();
        InitializeAreaOccupiedPercentage();
    }
    
    private void GenerateAreaDataObject()
    {
        for (var i = 0; i < areaDetailsList.Count; i++)
        {
            var areaDetail = areaDetailsList[i];
            // Area Data
            var area = Instantiate(areaDataPrefab, transform).GetComponent<AreaData>();
            area.GetComponent<NetworkObject>().Spawn();
            area.Initialize(i);

            // Area Manager
            _areaDataList.Add(area);
        }
    }
    
    private void InitializeAreaOccupiedPercentage()
    {
        _areaDataList[0].CallUpdateOccupiedPercentage(Team.Blue, 100); // SB
        _areaDataList[1].CallUpdateOccupiedPercentage(Team.Blue, 50); // AB
        _areaDataList[3].CallUpdateOccupiedPercentage(Team.Red, 50); // AR
        _areaDataList[4].CallUpdateOccupiedPercentage(Team.Red, 100); // SR 
    }    

    #endregion

    public Team GetWinnerTeam()
    {
        var blueOccupied = 0f;
        var redOccupied = 0f;
        
        foreach (var areaObject in GameObject.FindGameObjectsWithTag("AreaData")) // Client don't have _areaDataList
        {
            areaObject.TryGetComponent(out AreaData areaData);
            blueOccupied += areaData.blueTeamOccupiedPercentage.Value;
            redOccupied += areaData.redTeamOccupiedPercentage.Value; 
        }

        Debug.Log(blueOccupied + " " + redOccupied);
        return blueOccupied > redOccupied ? Team.Blue : Team.Red;
    }

}
