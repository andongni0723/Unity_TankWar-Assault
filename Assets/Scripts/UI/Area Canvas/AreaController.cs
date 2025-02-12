using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaController : MonoBehaviour
{
    [Header("Component")]
    private AreaData _areaData;
    private BoxCollider _boxCollider;
    
    public Timer blueTeamOccupiedCooldownTimer;
    public Timer redTeamOccupiedCooldownTimer;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.enabled = false;
    }
    
    public void Initialize(AreaData areaData, bool canOccupy)
    {
        _areaData = areaData;
        _boxCollider.enabled = canOccupy;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.name);
        if (!other.TryGetComponent(out CharacterController characterController)) return;
        Debug.Log("Player stay");
        switch (characterController.team.Value)
        {
            case Team.Blue:
                if (blueTeamOccupiedCooldownTimer.isPlay) return;
                _areaData.CallUpdateOccupiedPercentage(Team.Blue, 10);
                blueTeamOccupiedCooldownTimer.Play();
                break;
            case Team.Red:
                if (redTeamOccupiedCooldownTimer.isPlay) return;
                _areaData.CallUpdateOccupiedPercentage(Team.Red, 10);
                redTeamOccupiedCooldownTimer.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
}
