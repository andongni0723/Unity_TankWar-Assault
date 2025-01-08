using System;
using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [Header("Component")] 
    public CinemachineOrbitalFollow camOrbitalFollow;

    public GameObject camDirPoint;
    public Timer cameraUpdateTimer;
    
    private CharacterController _character;
    private CharacterController _enemyCharacter;

    [Header("Debug")]
    private float cameraAngle; 

    private void Awake()
    {
        _character = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        cameraUpdateTimer.OnTimerEnd += UpdateCameraOrbitHorizontalAxis;
    }

    private void OnDisable()
    {
        cameraUpdateTimer.OnTimerEnd -= UpdateCameraOrbitHorizontalAxis;
    }

    private void Update()
    {
        camDirPoint.transform.localRotation = Quaternion.Euler(0, 90, -camOrbitalFollow.transform.eulerAngles.x);
    }

    private void UpdateCameraOrbitHorizontalAxis()
    {
        if (!TryGetEnemyCharacter()) return;
        
        // Camera Look at Player to Enemy 
        var enemyDir = _enemyCharacter.transform.position - _character.transform.position;
        cameraAngle = Mathf.Atan2(enemyDir.x, enemyDir.z) * Mathf.Rad2Deg;
        DOTween.To(
            () => camOrbitalFollow.HorizontalAxis.Value, // 起始值
            value => camOrbitalFollow.HorizontalAxis.Value = value, // 更新值
            cameraAngle, // 目標值
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