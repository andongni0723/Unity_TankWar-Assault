using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterShoot : NetworkBehaviour
{
    [Header("Component")] 
    public GameObject firePoint;
    
    [Header("Settings")]
    public PoolKey bulletPoolKey = PoolKey.RedBullet;
    //[Header("Debug")]

    public void Shoot()
    {
        if (!IsOwner) return;
        var bullet = ObjectPoolManager.Instance.GetObject(bulletPoolKey);
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.rotation = firePoint.transform.rotation;
    }
}
