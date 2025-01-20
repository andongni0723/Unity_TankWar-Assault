using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponButton : MonoBehaviour
{
    [Header("Component")] 
    public TMP_Text buttonText;

    [Header("Settings")]
    public TankWeaponType weaponType;
    
    //[Header("Debug")]

    private void OnEnable()
    {
        var weaponID = weaponType == TankWeaponType.MainWeapon
            ? GameDataManager.Instance.tankMainWeaponDetails.weaponID
            : GameDataManager.Instance.tankSubWeaponDetails.weaponID;
        var weaponDetails = GameDataManager.Instance.UseWeaponIDGetWeaponDetails(weaponID);
        buttonText.text = weaponDetails.weaponName;
    }
}
