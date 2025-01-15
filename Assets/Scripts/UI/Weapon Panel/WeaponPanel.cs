using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPanel : MonoBehaviour
{
    [Header("Component")] 
    public GameObject mainWeaponSelectGroup;
    public GameObject subWeaponSelectGroup;

    //[Header("Settings")]
    
    [Header("Debug")]
    private TankWeaponType _currentOpenWeaponType;
    private string _currentSelectedWeaponID;
    
    private void OnEnable()
    {
        EventHandler.OnWeaponSelectToggleSelected += OnWeaponSelectToggleSelected;
    }
    
    private void OnDisable()
    {
        EventHandler.OnWeaponSelectToggleSelected -= OnWeaponSelectToggleSelected;
    }

    private void OnWeaponSelectToggleSelected(WeaponDetailsSO data)
    {
        _currentSelectedWeaponID = data.weaponID;
    }

    public void ShowMainWeaponSelectGroup()
    {
        mainWeaponSelectGroup.SetActive(true);
        subWeaponSelectGroup.SetActive(false);
        _currentOpenWeaponType = TankWeaponType.MainWeapon;
    }
    
    public void ShowSubWeaponSelectGroup()
    {
        mainWeaponSelectGroup.SetActive(false);
        subWeaponSelectGroup.SetActive(true);
        _currentOpenWeaponType = TankWeaponType.SubWeapon;
    }
    
    public void ConfirmWeapon()
    {
        GameDataManager.Instance.UpdateTankData(_currentOpenWeaponType, _currentSelectedWeaponID);
        mainWeaponSelectGroup.SetActive(false);
        subWeaponSelectGroup.SetActive(false);
    }
}
