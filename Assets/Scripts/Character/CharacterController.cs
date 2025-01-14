using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    private Slider _leftTrackSlider;
    private Slider _rightTrackSlider;
    private Button _shootButton;
    private VariableJoystick _headJoystick;
    
    private CharacterShoot _characterShoot;
    private CharacterSetColor _characterSetColor;
    private CharacterMouseHandler _characterMouseHandler;
    
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 90;
    public float gravityScale = 1f;
    private const float GRAVITY = -9.81f;
    public LayerMask groundLayer;
    public bool useMobileRotate;
    
    public float tankHeadRotateSpeed = 30;
    public float maxRotationSpeed = 360f;
    
    public NetworkVariable<Team> team = new(0, writePerm: NetworkVariableWritePermission.Owner);
    
    [Header("Debug")]
    private Vector2 _moveDirection;
    private Camera _mainCamera;
    private float _previousAngle;    // 上一幀搖桿角度
    private float _currentRotation;


    public override void OnNetworkSpawn() => NetworkSpawnInitial();
    private void Awake() => InitialComponent();
    // private void OnEnable() => MobileShootCoolDownEvent();
    
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
        _leftTrackSlider = GameUIManager.Instance.leftTrackSlider;
        _rightTrackSlider = GameUIManager.Instance.rightTrackSlider;
        _headJoystick = GameUIManager.Instance.tankHeadJoystick;
        _shootButton = GameUIManager.Instance.fireButton;
        _characterShoot = GetComponent<CharacterShoot>();
        _characterSetColor = GetComponent<CharacterSetColor>();
        _characterMouseHandler = GetComponent<CharacterMouseHandler>();
        _mainCamera = Camera.main;
        cameraConfiner3D.BoundingVolume = GameObject.FindWithTag("CameraBound").GetComponent<BoxCollider>();
        // _shootButton.onClick.AddListener(MobileCallShoot);
        
#if UNITY_EDITOR    
        InitialInputSystemBinding();
        DesktopMoveBinding();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        InitialInputSystemBinding();
        DesktopMoveBinding();
#elif UNITY_ANDROID || UNITY_IOS
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
        // _inputSystem.Player.Attack.performed += _ => DesktopCallShoot();
        _inputSystem.Player.Fire.performed += _ => MobileCallShoot();
        _inputSystem.Player.StopTank.performed += _ => StopTankMove();
        DragMouseBinding();
    }

    private void DesktopMoveBinding()
    {
        _inputSystem.Player.TankMoveLeft.performed += DesktopLeftTrackMovement;
        _inputSystem.Player.TankMoveRight.performed += DesktopRightTrackMovement;
    }

    private void DesktopLeftTrackMovement(InputAction.CallbackContext ctx)
    {
        _leftTrackSlider.value = ctx.control.name switch
        {
            "1" => 2,
            "q" => 1,
            "a" => 0,
            "z" => -1,
        };
    }
    
    private void DesktopRightTrackMovement(InputAction.CallbackContext ctx)
    {
        _rightTrackSlider.value = ctx.control.name switch
        {
            "3" => 2,
            "e" => 1,
            "d" => 0,
            "c" => -1,
        };
    }

    private void StopTankMove()
    {
        _leftTrackSlider.value = 0;
        _rightTrackSlider.value = 0;
    }

    private void DragMouseBinding()
    {
        _inputSystem.Player.MouseDrag.started += _ => _characterMouseHandler.OnMouseClickStarted();
        _inputSystem.Player.MouseDrag.canceled += _ => _characterMouseHandler.OnMouseClickCanceled();
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
        LeftAndRightSliderValueGetArrowDirection(_leftTrackSlider.value, _rightTrackSlider.value);
        UpdateTankHeadRotation(_headJoystick.Horizontal, _headJoystick.Vertical);
        FireButtonUIUpdate();
        // MobileMovement();
        // var dir = LeftAndRightSliderValueGetArrowDirection(_leftTrackSlider.value, _rightTrackSlider.value);
        // MobileMovementWithDirection(dir);
        // MobileRotate();
    }
    
    private void DesktopInputControl()
    {
        // DesktopMovement(_moveDirection);
        DesktopRotate();
    } 

    #endregion

    #region New Mobile Control
    private void LeftAndRightSliderValueGetArrowDirection(float leftTrack, float rightTrack)
    {
        leftTrack = Mathf.Clamp(leftTrack, -1f, 2f);
        rightTrack = Mathf.Clamp(rightTrack, -1f, 2f);

        // 計算坦克的前進方向和旋轉
        float forwardMovement = (leftTrack + rightTrack) / 2f; // 平均值決定前進或後退的速度
        float rotation = (rightTrack - leftTrack) / 2f;        // 差值決定旋轉的方向和速度

        _rb.linearVelocity = -tankBody.transform.forward  * (forwardMovement * moveSpeed);
        tankBody.transform.Rotate(0, -rotation * turnSpeed * Time.fixedDeltaTime, 0);
    }
    
    public void UpdateTankHeadRotation(float joystickX, float joystickY)
    {
        // 1. 當搖桿靜止時，維持當前旋轉角度
        if (joystickX == 0 && joystickY == 0) return;

        // 2. 計算當前搖桿角度（0° ~ 360°）
        float currentAngle = Mathf.Atan2(joystickX, joystickY) * Mathf.Rad2Deg;
        if (currentAngle < 0) currentAngle += 360;

        // 3. 計算角度變化量
        float deltaAngle = Mathf.DeltaAngle(_previousAngle, currentAngle);

        // 4. 調整旋轉速度，累積旋轉角度
        _currentRotation += Mathf.Clamp(deltaAngle, -maxRotationSpeed, maxRotationSpeed) * Time.deltaTime * tankHeadRotateSpeed;

        // 5. 使用 Quaternion.RotateTowards 進行旋轉
        Quaternion targetRotation = Quaternion.Euler(-90, _currentRotation, 0);
        tankHead.transform.rotation = Quaternion.RotateTowards(tankHead.transform.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);

        // 6. 更新上一幀角度
        _previousAngle = currentAngle;
    }

    private void FireButtonUIUpdate()
    {
        // fill amount 0 ~ 1
        _shootButton.GetComponent<Image>().fillAmount =
            mobileShootTimer.isPlay ? mobileShootTimer.currentTime / mobileShootTimer.time : 1;
    }
    
    private void MobileCallShoot()
    {
        if(mobileShootTimer.isPlay) return;
        _characterShoot.ExecuteShoot();
        mobileShootTimer.Play();
    }
    #endregion
    
    #region Desktop Control

    // private void DesktopMovement(Vector2 readValue)
    // {
    //     var dir = cameraDirPoint.transform.TransformDirection(new Vector3(-readValue.y, 0, readValue.x));
    //     _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, dir.normalized * moveSpeed, 
    //         Time.fixedDeltaTime * moveSpeed * 5);
    //     
    //     if(_moveDirection == Vector2.zero) return;
    //     var localDir = cameraDirPoint.transform.TransformDirection(readValue);
    //     float angleY = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg - 90;
    //     tankBody.transform.localRotation = Quaternion.Euler(0, angleY, 0);
    // }

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
