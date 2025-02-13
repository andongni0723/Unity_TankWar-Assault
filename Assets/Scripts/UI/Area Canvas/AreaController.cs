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
    private int _currentPlayerInArea;
    private bool _canOccupy;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.enabled = false;
    }
    
    private void OnEnable()
    {
        EventHandler.OnGameStart += OnGameStart;
    }
    
    private void OnDisable()
    {
        EventHandler.OnGameStart -= OnGameStart;
    }
    
    public void Initialize(AreaData areaData, bool canOccupy)
    {
        _areaData = areaData;
        _canOccupy = canOccupy;
    }

    private void OnGameStart()
    {
        _boxCollider.enabled = _canOccupy;
    }
    
    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.TryGetComponent(typeof(CharacterController), out _))
    //         _currentPlayerInArea++;
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     if(other.TryGetComponent(typeof(CharacterController), out _))
    //         _currentPlayerInArea--;
    // }

    private void OnTriggerStay(Collider other)
    {
        // Two or more players in the area or not player
        // if(_currentPlayerInArea > 1 && _areaData.blueTeamOccupiedPercentage.Value + _areaData.redTeamOccupiedPercentage.Value >= 99) return;
        if(_areaData == null) return;
        if(!other.TryGetComponent(out CharacterController characterController)) return;
        
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