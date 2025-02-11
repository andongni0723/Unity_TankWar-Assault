using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AreaData : NetworkBehaviour
{
    [Header("Component")]
    public AreaUI areaUI;
    
    [Header("Settings")] 
    public NetworkVariable<float> blueTeamOccupiedPercentage = new(0, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> redTeamOccupiedPercentage = new(0, writePerm: NetworkVariableWritePermission.Server);
    
    //[Header("Debug")]

    public override void OnNetworkSpawn()
    {
        blueTeamOccupiedPercentage.OnValueChanged += (oldValue, newValue) => areaUI.UpdateUI(this);
    }

    /// <summary>
    /// Sets the occupied percentage for the specified team and adjusts the other team's percentage accordingly.
    /// The total occupation will always sum to 100%.
    /// </summary>
    /// <param name="team"></param>
    /// <param name="percentage"></param>
    public void CallUpdateOccupiedPercentage(Team team, float percentage)
    {
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
        // if(percentage is < 0 or > 100) Debug.LogError("Invalid percentage value ({percentage}). The percentage must be between 0 and 100.");
        percentage = Mathf.Clamp(percentage, 0, 100);
        blueTeamOccupiedPercentage.Value = team == Team.Blue ?
            percentage : Mathf.Clamp(blueTeamOccupiedPercentage.Value, 0, 100 - percentage);
        redTeamOccupiedPercentage.Value = team == Team.Red ? 
            percentage : Mathf.Clamp(redTeamOccupiedPercentage.Value, 0, 100 - percentage);

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
