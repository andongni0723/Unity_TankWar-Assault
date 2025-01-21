using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Sirenix.Serialization;
using Random = UnityEngine.Random;

public class CharacterShoot : NetworkBehaviour
{
    [Header("Component")] 
    private CharacterController _cc;
    [SerializeField]private GameObject _currentFirePoint;
    
    [Header("Settings")]
    public NetworkVariable<PoolKey> mainWeaponBulletPoolKey = new(PoolKey.BlueBullet, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<PoolKey> subWeaponBulletPoolKey = new(PoolKey.MachineGunBullet, writePerm: NetworkVariableWritePermission.Owner);
    [OdinSerialize]public Dictionary<WeaponDetailsSO, GameObject> firePointDict = new();
    
    [Header("Debug")]
    public NetworkVariable<TankWeaponType> currentWeaponType = new(TankWeaponType.MainWeapon, writePerm: NetworkVariableWritePermission.Owner);
    private WeaponDetailsSO _currentWeaponDetail;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _currentWeaponDetail = GameDataManager.Instance.tankMainWeaponDetails;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        mainWeaponBulletPoolKey.Value = _cc.team.Value == Team.Blue? PoolKey.BlueBullet : PoolKey.RedBullet;
    }
    
    public void ChangeWeapon(TankWeaponType newWeaponType, WeaponDetailsSO newWeaponDetail)
    {
        // _currentWeaponType = newWeaponType;
        currentWeaponType.Value = newWeaponType;
        _currentWeaponDetail = newWeaponDetail;
        _cc.mobileShootTimer.time = _currentWeaponDetail.shootingInterval;
    }

    public void ExecuteShoot()
    {
        if(!IsOwner) return;

        var position = _currentFirePoint.transform.position;
        var rotation = _currentFirePoint.transform.rotation;
        ShootServerRpc(position, rotation);
        Shoot(position, rotation);
    }

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
        // var bulletPoolKey = _currentWeaponType == TankWeaponType.MainWeapon ? mainWeaponBulletPoolKey.Value : subWeaponBulletPoolKey.Value;
        var bulletPoolKey = currentWeaponType.Value == TankWeaponType.MainWeapon ? mainWeaponBulletPoolKey.Value : subWeaponBulletPoolKey.Value;
        var bullet = ObjectPoolManager.Instance.GetObject(bulletPoolKey).GetComponent<Bullet>();
        bullet.gameObject.tag = _cc.team.Value == Team.Blue ? "Blue Skill" : "Red Skill";
        bullet.gameObject.layer = _cc.team.Value == Team.Blue ? LayerMask.NameToLayer("Blue Skill") : LayerMask.NameToLayer("Red Skill");
        bullet.Initialize(pos, rot, Random.Range(-_currentWeaponDetail.spreadAngle, _currentWeaponDetail.spreadAngle));
    }
}
