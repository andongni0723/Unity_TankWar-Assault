using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
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
    public GameObject groundCanvas;
    public DecalProjector weaponShootArea;
    public DecalProjector projectileHitArea;
    public Timer stopEffectExtendTimer;
    
    private Rigidbody _rb;
    private PlayerInputControl _inputSystem;
    private VariableJoystick _moveJoystick;
    private Slider _leftTrackSlider;
    private Slider _rightTrackSlider;
    private VariableJoystick _headJoystick;
    private Button _fireButton;
    private Button _cancelFireButton;
    
    private CharacterShoot _characterShoot;
    private CharacterSetColor _characterSetColor;
    private CharacterMouseHandler _characterMouseHandler;
    private CharacterCameraController _characterCameraController;
    
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 90;
    public float gravityScale = 1f;
    private const float GRAVITY = -9.81f;
    public LayerMask groundLayer;
    public bool useMobileRotate;
    
    public NetworkVariable<Team> team = new(0, writePerm: NetworkVariableWritePermission.Owner);
    
    [Header("Debug")]
    public const float SHOOT_RADIUS_TO_SHOOT_AREA_SIZE = 10f;
    public const float HIT_RADIUS_TO_HIT_AREA_SIZE = 6.5f; // 13 / 2
    private Vector2 _moveDirection;
    private Camera _mainCamera;
    private float _previousAngle;    // 上一幀搖桿角度
    private float _currentRotation;


    public override void OnNetworkSpawn() => NetworkSpawnInitial();
    private void Awake() => InitialComponent();

    private void OnEnable()
    {
        EventHandler.OnPlayerDied += OnPlayerDied;
        EventHandler.OnPlayerRespawn += OnPlayerRespawn;
        _headJoystick.OnPointerDownEvent += OpenSkillIndicatorCheck;
        _headJoystick.OnPointerUpEvent += CheckStartFireWithJoystickRelease;
    }

    private void OnDisable()
    {
        EventHandler.OnPlayerDied -= OnPlayerDied;
        EventHandler.OnPlayerRespawn -= OnPlayerRespawn;
        _headJoystick.OnPointerDownEvent -= OpenSkillIndicatorCheck;
        _headJoystick.OnPointerUpEvent -= CheckStartFireWithJoystickRelease;
    }

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

    #region Event

    private void OnPlayerDied(bool isOwner, Timer respawnTimer)
    {
        if (isOwner)
            _inputSystem.Disable();
    }

    private void OnPlayerRespawn(bool isOwner)
    {
        if (isOwner)
            _inputSystem.Enable();
    }
    

    #endregion

    #region Initialize
    
    private void InitialComponent()
    {
        _rb = GetComponent<Rigidbody>();
        _inputSystem = new PlayerInputControl();
        _inputSystem.Enable();
        _leftTrackSlider = GameUIManager.Instance.leftTrackSlider;
        _rightTrackSlider = GameUIManager.Instance.rightTrackSlider;
        _headJoystick = GameUIManager.Instance.tankHeadJoystick;
        _fireButton = GameUIManager.Instance.fireButton;
        _cancelFireButton = GameUIManager.Instance.cancelFireButton;
        _characterShoot = GetComponent<CharacterShoot>();
        _characterSetColor = GetComponent<CharacterSetColor>();
        _characterMouseHandler = GetComponent<CharacterMouseHandler>();
        _characterCameraController = GetComponent<CharacterCameraController>();
        _mainCamera = Camera.main;
        CloseSkillIndicator();
        
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
            EventHandler.CallOnOwnerSpawned(this);
            InitialSpawnPoint();
            SetTeamLayerServerRpc(team.Value == Team.Blue ? LayerMask.NameToLayer("Blue Player") : LayerMask.NameToLayer("Red Player")); 
        }
        
        // Character Data Setting
        name = team.Value == Team.Blue ? "Blue Player" : "Red Player";
        virtualCamera.gameObject.SetActive(IsOwner);
        groundCanvas.SetActive(IsOwner);
        _characterSetColor.SetColorBasedOnOwner();
        stopEffectExtendTimer.time = GameDataManager.Instance.stopButtonEffectExpand ? 
            GameDataManager.Instance.startButtonExpandTime : 0;
        
        EventHandler.CallOnPlayerSpawned(this);
        
        // If you are the second player
        if(IsHost != IsOwner)
            EventHandler.CallOnAllPlayerSpawned();
    }

    private void InitialInputSystemBinding()
    {
        _inputSystem.Player.Fire.started += _ => 
        {
            switch (_characterShoot.currentWeaponData.weaponDetails.fireType)
            {
                case WeaponFireType.Direct:
                    _characterShoot.StartShoot();
                    break;
                case WeaponFireType.AOE:
                    break;
            }
        };
        _inputSystem.Player.Fire.canceled += _ =>
        {
            switch (_characterShoot.currentWeaponData.weaponDetails.fireType)
            {
                case WeaponFireType.Direct:
                    _characterShoot.StopShoot();
                    break;
            }
        };
        _inputSystem.Player.CancelFire.performed += _ =>
        {
            switch (_characterShoot.currentWeaponData.weaponDetails.fireType)
            {
                case WeaponFireType.AOE:
                    CloseSkillIndicator();
                    break;
            }
        };
        _inputSystem.Player.StopTank.performed += _ => StopTankMove(); 
        DragMouseBinding();
    }
    
    private void CheckStartFireWithJoystickRelease()
    {
        // Is AOE weapon and didn't press Cancel button before release joystick
        if (_characterShoot.currentWeaponData.weaponDetails.fireType == WeaponFireType.AOE && weaponShootArea.gameObject.activeSelf)
            _characterShoot.OneShoot();
        CloseSkillIndicator();
    }

    private void DesktopMoveBinding()
    {
        _inputSystem.Player.TankMoveLeft.performed += DesktopLeftTrackMovement;
        _inputSystem.Player.TankMoveRight.performed += DesktopRightTrackMovement;
    }

    private void DesktopLeftTrackMovement(InputAction.CallbackContext ctx)
    {
        if(stopEffectExtendTimer.isPlay) return;
        _leftTrackSlider.value = ctx.control.name switch
        {
            "1" => 2,
            "q" => 1,
            "a" or "s" => 0,
            "z" => -1,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private void DesktopRightTrackMovement(InputAction.CallbackContext ctx)
    {
        if(stopEffectExtendTimer.isPlay) return;
        _rightTrackSlider.value = ctx.control.name switch
        {
            "3" => 2,
            "e" => 1,
            "d" or "s" => 0,
            "c" => -1,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void StopTankMove() //TODO: Stop effect extend  
    {
        stopEffectExtendTimer.Play();
        _leftTrackSlider.value = 0;
        _rightTrackSlider.value = 0;
    }

    private void DragMouseBinding()
    {
        if(!GameDataManager.Instance.canDragCamera) return;
        _inputSystem.Player.MouseDrag.started += _ => _characterMouseHandler.OnMouseClickStarted();
        _inputSystem.Player.MouseDrag.canceled += _ => _characterMouseHandler.OnMouseClickCanceled();
    }

    public void InitialSpawnPoint()
    {
        if(!IsOwner) return;
        StartCoroutine(WaitInitialSpawnPoint());
    }
    
    private IEnumerator WaitInitialSpawnPoint()
    {
        yield return null;
        var spawnPoint = SpawnManager.Instance.GetStartSpawnPoint(team.Value);
        transform.position = spawnPoint.transform.position;
        tank.transform.rotation = spawnPoint.transform.rotation;
        _characterCameraController.UpdateCameraAngleWithStartByTeam();
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
        IndicatorAction();
    }
    
    private void DesktopInputControl()
    {
        DesktopRotate();
        IndicatorAction();
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
        tankHead.transform.Rotate(0, 0, -rotation * turnSpeed * Time.fixedDeltaTime);
    }

    private void UpdateTankHeadRotation(float joystickX, float joystickY)
    {
        if (_headJoystick.Horizontal == 0 && _headJoystick.Vertical == 0) return;
        
        // Head Rotate 
        var inputDir = new Vector3(_headJoystick.Horizontal, 0, _headJoystick.Vertical);
        var localDir = cameraDirPoint.transform.TransformDirection(inputDir);
        var addValue = team.Value == Team.Blue ? -90 : 90;
        float angleY = Mathf.Atan2(-localDir.z, localDir.x) * Mathf.Rad2Deg + addValue;
        tankHead.transform.localRotation = Quaternion.Euler(-90, angleY, 0); 
        // Debug.Log("Current Angle: " + currentAngle + " Previous Angle: " + _previousAngle + " Delta Angle: " + deltaAngle + " Current Rotation: " + _currentRotation);

    }
    
    #endregion
    
    #region Desktop Control

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
    #endregion

    #region Skill Indicator

    private void OpenSkillIndicatorCheck()
    {
        if(_characterShoot.currentWeaponData.weaponDetails.fireType == WeaponFireType.AOE)
            OpenSkillIndicator();
    }
    
    private void OpenSkillIndicator()
    {
        if (!IsOwner) return;
        weaponShootArea.gameObject.SetActive(true);
        projectileHitArea.gameObject.SetActive(true);
        _cancelFireButton.gameObject.SetActive(true);
        _fireButton.gameObject.SetActive(false);
        
        //Initial Indicator Size
        var shootRadius = _characterShoot.currentWeaponData.weaponDetails.shootingRadius * SHOOT_RADIUS_TO_SHOOT_AREA_SIZE;
        var hitRadius = _characterShoot.currentWeaponData.weaponDetails.hitRadius * HIT_RADIUS_TO_HIT_AREA_SIZE;
        weaponShootArea.size = new Vector3(shootRadius, shootRadius, 20f);
        projectileHitArea.size = new Vector3(hitRadius, hitRadius, 20f);
    }
    
    private void CloseSkillIndicator()
    {
        if (!IsOwner) return;
        weaponShootArea.gameObject.SetActive(false);
        projectileHitArea.gameObject.SetActive(false);
        _cancelFireButton.gameObject.SetActive(false);
        _fireButton.gameObject.SetActive(true);
    }
    
    private void IndicatorAction()
    {
        if (_headJoystick.Horizontal == 0 && _headJoystick.Vertical == 0) return;
        
        var shootRadius = _characterShoot.currentWeaponData.weaponDetails.shootingRadius * 10;
        var joystickPos = new Vector3(_headJoystick.Horizontal * shootRadius / 2f, _headJoystick.Vertical * shootRadius / 2f, 3);
        projectileHitArea.transform.localPosition = Vector3.zero + joystickPos;
    }
    #endregion
}
