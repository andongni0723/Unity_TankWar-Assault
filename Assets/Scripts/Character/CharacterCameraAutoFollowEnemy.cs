using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterCameraAutoFollowEnemy : CharacterCameraController
{
    [Header("Settings")]
    public bool isAutoFollowEnemy;
    public Timer cameraUpdateTimer;

    [Header("Debug")]
    private CharacterController _enemyCharacter;
    private float _cameraAngle;

    protected override void Awake()
    {
        base.Awake();
        isAutoFollowEnemy = GameDataManager.Instance.isAutoFollowEnemy;
        dragSpeed = GameDataManager.Instance.cameraDragSpeed;
        if (!isAutoFollowEnemy) enabled = false;
    }

    private void OnEnable()
    {
        if (!isAutoFollowEnemy) return;
        cameraUpdateTimer.OnTimerEnd += UpdateCameraOrbitHorizontalAxis;
    }
    private void OnDisable()
    {
        if (!isAutoFollowEnemy) return;
        cameraUpdateTimer.OnTimerEnd -= UpdateCameraOrbitHorizontalAxis;
    }
    
    private void UpdateCameraOrbitHorizontalAxis()
    {
        if (!TryGetEnemyCharacter()) return;
        
        // Camera Look at Player to Enemy 
        var enemyDir = _enemyCharacter.transform.position - _character.transform.position;
        _cameraAngle = Mathf.Atan2(enemyDir.x, enemyDir.z) * Mathf.Rad2Deg;
        var startAngle = camOrbitalFollow.HorizontalAxis.Value;
        var shortestAngle = Mathf.DeltaAngle(startAngle, _cameraAngle);
        var targetAngle = startAngle + shortestAngle;
        
        DOTween.To(
            () => startAngle, // 起始值
            value => camOrbitalFollow.HorizontalAxis.Value = value, // 更新值
            targetAngle, // 目標值
            3 // 過渡時間
        ).SetEase(Ease.InOutSine);
    }
    
    
    private bool TryGetEnemyCharacter()
    {
        if (_enemyCharacter != null) return true;
        _enemyCharacter = TeamManager.Instance.GetEnemyTeamCharacterController();
        return _enemyCharacter != null;
    }
}
