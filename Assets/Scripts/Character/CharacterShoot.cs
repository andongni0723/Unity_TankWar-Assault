using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using Random = UnityEngine.Random;

public class CharacterShoot : NetworkBehaviour
{
    [Header("Component")] 
    private CharacterController _cc;
    [SerializeField]private GameObject _currentFirePoint;
    
    [Header("Settings")]
    public NetworkVariable<PoolKey> mainWeaponBulletPoolKey = new(PoolKey.BlueBullet, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<PoolKey> subWeaponBulletPoolKey = new(PoolKey.MachineGunBullet, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString32Bytes> mainWeaponID = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString32Bytes> subWeaponID = new(writePerm: NetworkVariableWritePermission.Owner);
    
    // [OdinSerialize]public Dictionary<WeaponDetailsSO, GameObject> firePointDict = new();
    public GameWeaponData mainWeaponData;
    public GameWeaponData subWeaponData;
    public GameWeaponData currentWeaponData { get; private set; }

    [Header("Debug")]
    public NetworkVariable<TankWeaponType> currentWeaponType = new(TankWeaponType.MainWeapon, writePerm: NetworkVariableWritePermission.Owner);
    private bool _isShooting;

    private void Awake()
    {
        InitializeSetting();
    }

    public override void OnNetworkSpawn()
    {
        currentWeaponType.OnValueChanged += (pre, now) => 
            currentWeaponData = GetCurrentWeaponData(now);

        StartCoroutine(NetworkInitial());
    }

    private void Update()
    {
        ExecuteShoot();
    }

    #region Initialization

    private void InitializeSetting()
    {
        _cc = GetComponent<CharacterController>();
        currentWeaponData = mainWeaponData;
    }
    
    private IEnumerator NetworkInitial()
    {
        if (IsOwner) SettingWeaponID();
        yield return new WaitUntil(() => !mainWeaponID.Value.IsEmpty && !subWeaponID.Value.IsEmpty);
        SettingWeaponData(mainWeaponData, GameDataManager.Instance.UseWeaponIDGetWeaponDetails(mainWeaponID.Value.ToString()));
        SettingWeaponData(subWeaponData, GameDataManager.Instance.UseWeaponIDGetWeaponDetails(subWeaponID.Value.ToString()));
        if (IsOwner) SettingProjectilePoolKey();
    }

    private void SettingWeaponID()
    {
        if (!IsOwner) return;
        mainWeaponID.Value = GameDataManager.Instance.tankMainWeaponDetails.weaponID;
        subWeaponID.Value = GameDataManager.Instance.tankSubWeaponDetails.weaponID;
    }
    
    private void SettingProjectilePoolKey()
    {
        if (!IsOwner) return;
        mainWeaponBulletPoolKey.Value = _cc.team.Value == Team.Blue? 
            mainWeaponData.weaponDetails.projectileDetails.bluePrefabPoolKey : 
            mainWeaponData.weaponDetails.projectileDetails.redPrefabPoolKey;
        
        subWeaponBulletPoolKey.Value = _cc.team.Value == Team.Blue?
            subWeaponData.weaponDetails.projectileDetails.bluePrefabPoolKey :
            subWeaponData.weaponDetails.projectileDetails.redPrefabPoolKey; 
    }

    private void SettingWeaponData(GameWeaponData weaponData, WeaponDetailsSO weaponDetails)
    {
        weaponData.weaponDetails = weaponDetails;
        weaponData.shootTimer.time = weaponDetails.shootingInterval;
        weaponData.reloadTimer.time = weaponDetails.reloadTime;
        weaponData.currentAmmo = weaponDetails.capacity;
        weaponData.reloadTimer.OnTimerEnd += () => OnReloadTimerEnd(weaponData);
    }

    private void OnReloadTimerEnd(GameWeaponData weaponData)
    {
        weaponData.currentAmmo = weaponData.weaponDetails.capacity;
    }
    
    #endregion
    
    /// <summary>
    /// Call All Weapon ammo to full
    /// </summary>
    public void CallAllWeaponReload()
    {
        OnReloadTimerEnd(mainWeaponData);
        OnReloadTimerEnd(subWeaponData);
    }

    #region Change Weapon
    public void ChangeWeapon(TankWeaponType newWeaponType, WeaponDetailsSO newWeaponDetail)
    {
        ChangeWeaponAction(newWeaponType);
        ChangeWeaponServerRpc(newWeaponType);
    }

    private void ChangeWeaponAction(TankWeaponType newWeaponType)
    {
        currentWeaponData.reloadTimer.Play();
        if(IsOwner) currentWeaponType.Value = newWeaponType;
        currentWeaponData.reloadTimer.Pause();
    }
    
    
    private GameWeaponData GetCurrentWeaponData(TankWeaponType weaponType) => 
        weaponType == TankWeaponType.MainWeapon ? mainWeaponData : subWeaponData;
    private GameWeaponData GetCurrentWeaponData() => GetCurrentWeaponData(currentWeaponType.Value); 
    
    [ServerRpc]
    private void ChangeWeaponServerRpc(TankWeaponType newWeaponType)
    {
        ChangeWeaponClientRpc(newWeaponType);
    }
    
    [ClientRpc]
    private void ChangeWeaponClientRpc(TankWeaponType newWeaponType)
    {
        if(!IsOwner) ChangeWeaponAction(newWeaponType);
    }    
    #endregion
    
    #region Shoot
    public void StartShoot() => _isShooting = true;
    public void StopShoot() => _isShooting = false;
    
    public void OneShoot()
    {
        _isShooting = true;
        ExecuteShoot();
        _isShooting = false;
    }
    
    private void ExecuteShoot()
    {
        if(!IsOwner || !_isShooting || currentWeaponData.shootTimer.isPlay || !CheckEnoughAmmo()) return;
        currentWeaponData.shootTimer.Play();
        var position = _currentFirePoint.transform.position;
        var targetPosition = _cc.projectileHitArea.transform.position; 
        var rotation = _currentFirePoint.transform.rotation;
        switch (currentWeaponData.weaponDetails.fireType)
        {
            case WeaponFireType.Direct:
                ShootServerRpc(position, rotation);
                Shoot(position, rotation);
                break;
            case WeaponFireType.AOE:
                ShootAOEServerRpc(position, targetPosition);
                ShootAOE(position, targetPosition);
                break;
            case WeaponFireType.Point:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private bool CheckEnoughAmmo() => currentWeaponData.currentAmmo > 0 || currentWeaponData.weaponDetails.infiniteAmmo;

    [ServerRpc]
    private void ShootServerRpc(Vector3 pos, Quaternion rot)
    {
        ShootClientRpc(pos, rot);
    }
    
    [ClientRpc]
    private void ShootClientRpc(Vector3 pos, Quaternion rot)
    {
        if(!IsOwner) Shoot(pos, rot);
    }

    private void Shoot(Vector3 pos, Quaternion rot)
    {
        var bulletPoolKey = currentWeaponType.Value == TankWeaponType.MainWeapon ? mainWeaponBulletPoolKey.Value : subWeaponBulletPoolKey.Value;
        var bullet = ObjectPoolManager.Instance.GetObject(bulletPoolKey).GetComponent<Bullet>();
        bullet.gameObject.tag = _cc.team.Value == Team.Blue ? "Blue Skill" : "Red Skill";
        bullet.gameObject.layer = _cc.team.Value == Team.Blue ? LayerMask.NameToLayer("Blue Skill") : LayerMask.NameToLayer("Red Skill");
        bullet.Initialize(pos, rot, Random.Range(-currentWeaponData.weaponDetails.spreadAngle, currentWeaponData.weaponDetails.spreadAngle));
        EventHandler.CallCameraShake(5, 0.1f);
        if(!currentWeaponData.weaponDetails.infiniteAmmo) currentWeaponData.currentAmmo--;
    }
    
    [ServerRpc]
    private void ShootAOEServerRpc(Vector3 startPos, Vector3 targetPos)
    {
        ShootAOEClientRpc(startPos, targetPos);
    }
    
    [ClientRpc]
    private void ShootAOEClientRpc(Vector3 startPos, Vector3 targetPos)
    {
        if(!IsOwner) ShootAOE(startPos, targetPos);
    }


    private void ShootAOE(Vector3 startPos, Vector3 targetPos)
    {
        var bulletPoolKey = currentWeaponType.Value == TankWeaponType.MainWeapon ? mainWeaponBulletPoolKey.Value : subWeaponBulletPoolKey.Value;
        var bullet = ObjectPoolManager.Instance.GetObject(bulletPoolKey).GetComponent<Bullet>();
        bullet.gameObject.tag = _cc.team.Value == Team.Blue ? "Blue Skill" : "Red Skill";
        bullet.gameObject.layer = _cc.team.Value == Team.Blue ? LayerMask.NameToLayer("Blue Skill") : LayerMask.NameToLayer("Red Skill");
        bullet.Initialize(startPos, targetPos, Random.Range(-currentWeaponData.weaponDetails.spreadAngle, currentWeaponData.weaponDetails.spreadAngle));
        if(!currentWeaponData.weaponDetails.infiniteAmmo) currentWeaponData.currentAmmo--; 
    }
    #endregion


}
