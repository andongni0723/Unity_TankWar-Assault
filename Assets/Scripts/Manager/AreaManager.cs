using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class AreaDetails
{
    public string areaName;
    public AreaController areaController;
    public AreaUI areaUI;
}

public class AreaManager : Singleton<AreaManager>
{
    [Header("Component")]
    public GameObject areaDataPrefab;
    
    [Header("Settings")]
    public List<AreaDetails> areaDetailsList = new();
    // public List<string> areaNameList = new();
    // public List<AreaController> areaControllerList = new();
    // public List<AreaUI> areaUIList = new();
    
    private List<AreaData> _areaDataList = new();
    
    //[Header("Debug")]

    private void OnEnable()
    {
        EventHandler.OnAllPlayerSpawned += OnAllPlayerSpawned;
    }

    private void OnAllPlayerSpawned()
    {
        // if(!NetworkManager.Singleton.IsServer) return;
        // GenerateAreaDataObjectSeverRpc();
        if (!NetworkManager.Singleton.IsServer) return;
        GenerateAreaDataObject();
        InitializeAreaOccupiedPercentage();
    }
    //
    // [ServerRpc]
    // private void GenerateAreaDataObjectSeverRpc()
    // {
    //     GenerateAreaDataObjectClientRpc();
    // }
    //
    // [ClientRpc]
    // private void GenerateAreaDataObjectClientRpc()
    // {
    //     if(!NetworkManager.Singleton.IsServer) GenerateAreaDataObject();
    // }

    private void GenerateAreaDataObject()
    {
        var i = 0;
        
        foreach (var areaDetail in areaDetailsList)
        {
            // Area Data
            // var area = NetworkObject.InstantiateAndSpawn(areaDataPrefab, NetworkManager.Singleton).GetComponent<AreaData>();
            var area = Instantiate(areaDataPrefab, transform).GetComponent<AreaData>();
            area.GetComponent<NetworkObject>().Spawn();
            area.Initialize(i);
            
            // Area Manager
            _areaDataList.Add(area);
            
            // Area Controller
            // areaControllerList[i].Initialize(area, !areaNameList[i].Contains("S")); // Spawn Area can't occupied
            i++;
        }
    }
    
    private void InitializeAreaOccupiedPercentage()
    {
        _areaDataList[0].CallUpdateOccupiedPercentage(Team.Blue, 100); // SB
        _areaDataList[1].CallUpdateOccupiedPercentage(Team.Blue, 50); // AB
        _areaDataList[3].CallUpdateOccupiedPercentage(Team.Red, 50); // AR
        _areaDataList[4].CallUpdateOccupiedPercentage(Team.Red, 100); // SR 
        // var i = 0;
        // foreach (var areaDetails in areaDetailsList)
        // {
        //     areaDetails.areaUI.UpdateUI(_areaDataList[i]);
        //     i++;
        // }
    }
}
