using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AreaData : NetworkBehaviour
{
    [Header("Component")]
    private List<AreaUI> _areaUIList;
    
    [Header("Settings")] 
    public NetworkVariable<AreaName> areaName = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> areaID = new(-1, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> blueTeamOccupiedPercentage = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> redTeamOccupiedPercentage = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> towerSpawned = new(false, writePerm: NetworkVariableWritePermission.Server);
    
    //[Header("Debug")]

    public override void OnNetworkSpawn() => StartCoroutine(SpawnInitialize());

    private IEnumerator SpawnInitialize()
    {
        yield return new WaitUntil(() => areaID.Value != -1); // Wait until the areaID is set
        
        var areaDetails = AreaManager.Instance.areaDetailsList[areaID.Value];
        gameObject.name = areaDetails.areaName.ToString();
        _areaUIList = areaDetails.AreaUIList;
        areaDetails.areaController.Initialize(this, !areaDetails.areaName.ToString().Contains("S"));
        blueTeamOccupiedPercentage.OnValueChanged += (oldValue, newValue) => CallUpdateAreaUI();
        redTeamOccupiedPercentage.OnValueChanged += (oldValue, newValue) => CallUpdateAreaUI();
        CallUpdateAreaUI();
    }
    
    private void OnEnable()
    {
        EventHandler.OnEnergyTowerDestroyed += OnEnergyTowerDestroyed;
    }
    
    private void OnDisable()
    {
        EventHandler.OnEnergyTowerDestroyed -= OnEnergyTowerDestroyed;
    }

    public void Initialize(int id)
    {
        if (!IsServer) return;
        areaName.Value = AreaManager.Instance.areaDetailsList[id].areaName;
        areaID.Value = id;
    }
    
    public void CallTowerSpawned()
    {
        if (IsServer)
            towerSpawned.Value = true;
        else
            TowerSpawnedServerRpc();
    }
    
    [ServerRpc]
    private void TowerSpawnedServerRpc()
    {
        towerSpawned.Value = true;
    }
    
    private void OnEnergyTowerDestroyed(AreaName targetAreaName)
    {
        if (!IsServer || areaName.Value != targetAreaName) return;
        towerSpawned.Value = false;
    }

    private void CallUpdateAreaUI()
    {
        // _areaUI.UpdateUI(this);
        foreach (var areaUI in _areaUIList)
        {
            areaUI.UpdateUI(this);
        }
    }

    /// <summary>
    /// Sets the occupied percentage for the specified team and adjusts the other team's percentage accordingly.
    /// The total occupation will always sum to 100%.
    /// </summary>
    /// <param name="team"></param>
    /// <param name="percentage"></param>
    public void CallUpdateOccupiedPercentage(Team team, float percentage)
    {
        EventHandler.CallOnAreaOccupied(team, areaName.Value);
        
        if(IsServer) 
            UpdateOccupiedPercentageAction(team, percentage);
        else 
            UpdateOccupiedPercentageServerRpc(team, percentage);
    }

    [ServerRpc]
    private void UpdateOccupiedPercentageServerRpc(Team team, float percentage)
    {
        UpdateOccupiedPercentageAction(team, percentage);
    }

    private void UpdateOccupiedPercentageAction(Team team, float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);
        
        if (team == Team.Blue)
        {
            blueTeamOccupiedPercentage.Value = Mathf.Clamp(blueTeamOccupiedPercentage.Value + percentage, 0, 100);
            if(blueTeamOccupiedPercentage.Value + redTeamOccupiedPercentage.Value > 100)
                redTeamOccupiedPercentage.Value = 100 - blueTeamOccupiedPercentage.Value;
        }
        else
        {
            redTeamOccupiedPercentage.Value = Mathf.Clamp(redTeamOccupiedPercentage.Value + percentage, 0, 100);
            if (blueTeamOccupiedPercentage.Value + redTeamOccupiedPercentage.Value > 100)
                blueTeamOccupiedPercentage.Value = 100 - redTeamOccupiedPercentage.Value;
        }
    }
}
