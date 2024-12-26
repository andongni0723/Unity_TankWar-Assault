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
    public GameObject bulletPrefab;
    //[Header("Settings")]
    //[Header("Debug")]

    public void Shoot()
    {
        if (!IsOwner) return;
        var bullet = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
    }
}
