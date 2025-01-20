using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{
    [Header("Component")]
    public WeaponDetailsSO weaponDetail;
    public GameObject firePoint;
    
    [Header("Settings")]
    public TankWeaponType weaponType = TankWeaponType.MainWeapon;
    //[Header("Debug")]
}
