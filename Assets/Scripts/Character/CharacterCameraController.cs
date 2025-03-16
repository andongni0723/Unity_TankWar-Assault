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
    
    protected CharacterController _character;
    private CharacterMouseHandler _mouseHandler;

    [Header("Settings")] 
    public float dragSpeed = 2;
    
    [Header("Debug")]
    private float cameraAngle; 

    protected virtual void Awake()
    {
        _character = GetComponent<CharacterController>();
        _mouseHandler = GetComponent<CharacterMouseHandler>();
        dragSpeed = GameDataManager.Instance.cameraDragSpeed;
    }
    
    private void Update()
    {
        UpdateCameraOrbitHorizontalAxisWithMouseDrag();
        camDirPoint.transform.localRotation = Quaternion.Euler(0, 90, -camOrbitalFollow.transform.eulerAngles.x);
    }
    
    /// <summary>
    /// Initial the camera angle with the team
    /// </summary>
    public void UpdateCameraAngleWithStartByTeam()
    {
        camOrbitalFollow.HorizontalAxis.Value = _character.team.Value == Team.Blue ? 90 : -90;
    }

    private void UpdateCameraOrbitHorizontalAxisWithMouseDrag()
    {
        if (!_mouseHandler.isDragging) return;
        var delta = GameDataManager.Instance.isReverseX ? _mouseHandler.mouseDelta.x : -_mouseHandler.mouseDelta.x;
        camOrbitalFollow.HorizontalAxis.Value += delta * dragSpeed;
    }
}