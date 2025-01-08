using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CharacterController : NetworkBehaviour
{
    [Header("Component")]
    public CinemachineCamera virtualCamera;
    public CinemachineConfiner3D cameraConfiner3D;
    public GameObject cameraDirPoint;
    public GameObject tankHead;
    public GameObject tankBody;
    public GameObject tank;
    public Timer mobileShootTimer;
    
    private Rigidbody _rb;
    private PlayerInputControl _inputSystem;
    private VariableJoystick _moveJoystick;
    private VariableJoystick _headJoystick;
    
    private CharacterShoot _characterShoot;
    private CharacterSetColor _characterSetColor;
    private CharacterCameraController _characterCameraController;
    
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float gravityScale = 1f;
    private const float GRAVITY = -9.81f;
    public LayerMask groundLayer;
    public bool useMobileRotate;
    
    public NetworkVariable<Team> team = new(0, writePerm: NetworkVariableWritePermission.Owner);
    
    [Header("Debug")]
    private Vector2 _moveDirection;
    private Camera _mainCamera;

    public override void OnNetworkSpawn() => NetworkSpawnInitial();
    private void Awake() => InitialComponent();
    private void OnEnable() => MobileShootCoolDownEvent();
    
    public override void OnDestroy()
    {
        _inputSystem.Disable();
        base.OnDestroy();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        
        // Variable Update
        _moveDirection = _inputSystem.Player.Move.ReadValue<Vector2>();
        _rb.AddForce(GRAVITY * gravityScale * Vector3.up, ForceMode.Acceleration);

        // Player Input Control
        HandlePlatformInputControl();
    }

    #region Initialize
    
    private void InitialComponent()
    {
        _rb = GetComponent<Rigidbody>();
        _inputSystem = new PlayerInputControl();
        _inputSystem.Enable();
        _moveJoystick = GameObject.FindWithTag("MoveJoystick").GetComponent<VariableJoystick>();
        _headJoystick = GameObject.FindWithTag("HeadJoystick").GetComponent<VariableJoystick>();
        _characterShoot = GetComponent<CharacterShoot>();
        _characterSetColor = GetComponent<CharacterSetColor>();
        _characterCameraController = GetComponent<CharacterCameraController>();
        _mainCamera = Camera.main;
        cameraConfiner3D.BoundingVolume = GameObject.FindWithTag("CameraBound").GetComponent<BoxCollider>();
        
        #if UNITY_EDITOR    
            if (!useMobileRotate) InitialInputSystemBinding();
        #elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            InitialInputSystemBinding();
        #endif
    }
    
    private void NetworkSpawnInitial()
    {
        if (IsOwner)
        {
            Debug.LogWarning($"isHost: {IsHost}, isServer: {IsServer}, isClient: {IsClient}");
            team.Value = IsHost ? Team.Blue : Team.Red;
            name = team.Value == Team.Blue ? "Blue Player" : "Red Player";
            EventHandler.CallOnOwnerSpawned(this);
            InitialSpawnPoint();
            SetTeamLayerServerRpc(team.Value == Team.Blue ? LayerMask.NameToLayer("Blue Player") : LayerMask.NameToLayer("Red Player")); 
        }
        virtualCamera.gameObject.SetActive(IsOwner);
        EventHandler.CallOnPlayerSpawned(this);
        _characterSetColor.SetColorBasedOnOwner();
    }

    private void InitialInputSystemBinding()
    {
        _inputSystem.Player.Attack.performed += _ => DesktopCallShoot(); 
    }

    private void InitialSpawnPoint()
    {
        if (IsServer)
        {
            transform.position = GameObject.FindWithTag("SpawnPoint").transform.position;
            tank.transform.rotation = GameObject.FindWithTag("SpawnPoint").transform.rotation;
        }
    }
    
    [ServerRpc]
    private void SetTeamLayerServerRpc(int layer)
    {
        SetTeamLayerClientRpc(layer);
    }

    [ClientRpc]
    private void SetTeamLayerClientRpc(int layer)
    {
        gameObject.layer = layer;
        tank.layer = layer;
    }

    #endregion

    #region Platfrom Input Control

    private void HandlePlatformInputControl()
    {
        #if UNITY_EDITOR    
            if (useMobileRotate) MobileInputControl();
            else DesktopInputControl();
        #elif UNITY_ANDROID || UNITY_IOS
            MobileInputControl();
        #elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            DesktopInputControl();
        #endif 
    }

    private void MobileInputControl()
    {
        MobileMovement();
        MobileRotate();
    }
    
    private void DesktopInputControl()
    {
        DesktopMovement(_moveDirection);
        DesktopRotate();
    } 

    #endregion

    #region Mobile Control
    private void MobileMovement()
    {
        if (_moveJoystick.Horizontal == 0 && _moveJoystick.Vertical == 0) return;
        
        // Move
        var dir = cameraDirPoint.transform.TransformDirection(new Vector3(-_moveJoystick.Vertical, 0, _moveJoystick.Horizontal));
        _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, dir.normalized * moveSpeed, 
            Time.fixedDeltaTime * moveSpeed * 5);
        
        // Body Rotate
        var inputDir = new Vector3(_moveJoystick.Horizontal, 0, _moveJoystick.Vertical);
        var localDir = cameraDirPoint.transform.TransformDirection(inputDir);
        float angleY = Mathf.Atan2(-localDir.z, localDir.x) * Mathf.Rad2Deg - 90;
        tankBody.transform.localRotation = Quaternion.Euler(0, angleY, 0);  
    }

    private void MobileRotate()
    {
        if (_headJoystick.Horizontal == 0 && _headJoystick.Vertical == 0) return;
        
        // Head Rotate 
        var inputDir = new Vector3(_headJoystick.Horizontal, 0, _headJoystick.Vertical);
        var localDir = cameraDirPoint.transform.TransformDirection(inputDir);
        float angleY = Mathf.Atan2(-localDir.z, localDir.x) * Mathf.Rad2Deg - 90;
        tankHead.transform.localRotation = Quaternion.Euler(-90, angleY, 0); 
    }
    
    private void MobileShootCoolDownEvent() => mobileShootTimer.OnTimerEnd += MobileCallShoot;
    
    private void MobileCallShoot()
    {
        if (_headJoystick.Horizontal == 0 && _headJoystick.Vertical == 0) return;
        _characterShoot.ExecuteShoot();
    }
    #endregion

    #region Desktop Control

    private void DesktopMovement(Vector2 readValue)
    {
        var dir = cameraDirPoint.transform.TransformDirection(new Vector3(-readValue.y, 0, readValue.x));
        _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, dir.normalized * moveSpeed, 
            Time.fixedDeltaTime * moveSpeed * 5);
        
        if(_moveDirection == Vector2.zero) return;
        var localDir = cameraDirPoint.transform.TransformDirection(readValue);
        float angleY = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg - 90;
        tankBody.transform.localRotation = Quaternion.Euler(0, angleY, 0);
    }

    private void DesktopRotate()
    {
        var ray = _mainCamera.ScreenPointToRay(_inputSystem.Player.MousePosition.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 direction = hit.point - transform.position;
            direction.y = 0;
            float angleY = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg + 90;
            tankHead.transform.rotation = Quaternion.Euler(-90, -angleY, 0);
        }
    }
    
    private void DesktopCallShoot()
    {
        _characterShoot.ExecuteShoot();
    }
    #endregion
}
