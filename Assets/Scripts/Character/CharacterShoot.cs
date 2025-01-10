using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterShoot : NetworkBehaviour
{
    [Header("Component")] 
    private CharacterController _cc;
    public GameObject firePoint;
    
    [Header("Settings")]
    public NetworkVariable<PoolKey> bulletPoolKey = new(PoolKey.BlueBullet, writePerm: NetworkVariableWritePermission.Owner);
    // public PoolKey bulletPoolKey = PoolKey.BlueBullet;
    //[Header("Debug")]

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        bulletPoolKey.Value = _cc.team.Value == Team.Blue? PoolKey.BlueBullet : PoolKey.RedBullet;
    }

    public void ExecuteShoot()
    {
        if(!IsOwner) return;

        var position = firePoint.transform.position;
        var rotation = firePoint.transform.rotation;
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
        if(!IsOwner)
            Shoot(pos, rot);
    }

    private void Shoot(Vector3 pos, Quaternion rot)
    {
        var bullet = ObjectPoolManager.Instance.GetObject(bulletPoolKey.Value).GetComponent<Bullet>();
        bullet.Initialize(pos, rot);
        // bullet.transform.position = pos;
        // bullet.transform.rotation = rot;
        // Debug.Log(firePoint.transform.rotation);
    }
}
