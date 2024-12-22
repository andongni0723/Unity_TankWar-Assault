using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : NetworkBehaviour
{
    [Header("Component")]
    public CinemachineCamera virtualCamera;
    public CinemachineConfiner3D cameraConfiner3D;
    private Rigidbody _rb;
    private PlayerInputControl _inputSystem;
    
    [Header("Settings")]
    public float moveSpeed = 5f;
    
    [Header("Debug")]
    private Vector2 _moveDirection;

    public override void OnNetworkSpawn()
    {
        virtualCamera.gameObject.SetActive(IsOwner);
        if(!IsOwner) return;
        Debug.LogWarning($"isHost: {IsHost}, isServer: {IsServer}, isClient: {IsClient}");
        StartSetLocation();
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputSystem = new PlayerInputControl();
        _inputSystem.Enable();
        cameraConfiner3D.BoundingVolume = GameObject.FindWithTag("CameraBound").GetComponent<BoxCollider>();
        
        if(!IsHost)
            transform.position += Vector3.forward * 2;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        _moveDirection = _inputSystem.Player.Move.ReadValue<Vector2>();
        Movement(_moveDirection);
    }

    private void StartSetLocation()
    {
        transform.position = IsServer ? transform.position : new Vector3(0, 0.04f, 0);
        transform.rotation = IsServer ? transform.rotation : Quaternion.Euler(0, 180, 0);
    }

    private void Movement(Vector2 readValue)
    {
        var dir = transform.TransformDirection(new Vector3(-readValue.y, 0, readValue.x));
        _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, dir.normalized * moveSpeed, 
            Time.fixedDeltaTime * moveSpeed * 5);
    }
}
