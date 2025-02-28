using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaController : MonoBehaviour
{
    [Header("Component")]
    private AreaData _areaData;
    private BoxCollider _boxCollider;
    
    // public EnergyTower energyTower;
    public GameObject energyTowerPrefab;
    public Timer blueTeamOccupiedCooldownTimer;
    public Timer redTeamOccupiedCooldownTimer;
    public Timer towerSpawnedCooldownTimer;
    
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

    /// <summary>
    /// Call by AreaData
    /// </summary>
    /// <param name="areaData"></param>
    /// <param name="canOccupy"></param>
    public void Initialize(AreaData areaData, bool canOccupy)
    {
        _areaData = areaData;
        _canOccupy = canOccupy;
    }

    private void OnGameStart()
    {
        _boxCollider.enabled = _canOccupy;
    }
    
    private void GenerateEnergyTower(Team team)
    {
        if(!NetworkManager.Singleton.IsServer) return;
        if(towerSpawnedCooldownTimer.isPlay) return;
        if (_areaData.towerSpawned.Value) return;
        
        _areaData.CallTowerSpawned();
        towerSpawnedCooldownTimer.Play();
        var energyTower = Instantiate(energyTowerPrefab, transform.position, Quaternion.identity);
        energyTower.GetComponent<NetworkObject>().Spawn();
        energyTower.GetComponent<EnergyTower>().Initial(team, _areaData.areaName.Value);
    }
    

    private void OnTriggerStay(Collider other)
    {
        if(_areaData == null) return;                                                   // AreaData not initialized
        if(!other.TryGetComponent(out CharacterController characterController)) return; // Not Player
        if(_areaData.towerSpawned.Value) return;                                        // Tower Still exist
        
        switch (characterController.team.Value)
        {
            case Team.Blue:
                ProcessTeamOccupancy(Team.Blue, blueTeamOccupiedCooldownTimer, _areaData.blueTeamOccupiedPercentage.Value);
                break;
            
            case Team.Red:
                ProcessTeamOccupancy(Team.Red, redTeamOccupiedCooldownTimer, _areaData.redTeamOccupiedPercentage.Value);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// 處理指定隊伍的區域佔領邏輯，包含冷卻檢查、佔領數值更新及塔生成判斷
    /// </summary>
    /// <param name="team">玩家隊伍</param>
    /// <param name="cooldownTimer">對應的佔領冷卻計時器</param>
    /// <param name="currentOccupied">當前佔領百分比</param>
    private void ProcessTeamOccupancy(Team team, Timer cooldownTimer, float currentOccupied)
    {
        // cooldown or already occupied
        if (cooldownTimer.isPlay || currentOccupied >= 100) return;

        // update occupied percentage
        _areaData.CallUpdateOccupiedPercentage(team, 10);
        cooldownTimer.Play();

        // get updated occupied percentage
        float updatedOccupied = team == Team.Blue ? 
            _areaData.blueTeamOccupiedPercentage.Value : _areaData.redTeamOccupiedPercentage.Value;

        // Check not occupied all and tower not exist and not spawn point
        if (updatedOccupied >= 100 && _canOccupy && !_areaData.towerSpawned.Value)
        {
            GenerateEnergyTower(team);
        }
    }
    
}