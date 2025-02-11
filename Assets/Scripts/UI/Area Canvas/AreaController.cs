using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(AreaData))]
public class AreaController : MonoBehaviour
{
    [Header("Component")]
    private AreaData areaData;
    public Timer blueTeamOccupiedCooldownTimer;
    public Timer redTeamOccupiedCooldownTimer;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        areaData = GetComponent<AreaData>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out CharacterController characterController)) return;
        switch (characterController.team.Value)
        {
            case Team.Blue:
                if (blueTeamOccupiedCooldownTimer.isPlay) return;
                areaData.CallUpdateOccupiedPercentage(Team.Blue, 10);
                blueTeamOccupiedCooldownTimer.Play();
                break;
            case Team.Red:
                if (redTeamOccupiedCooldownTimer.isPlay) return;
                areaData.CallUpdateOccupiedPercentage(Team.Red, 10);
                redTeamOccupiedCooldownTimer.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
}
