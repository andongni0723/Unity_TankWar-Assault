using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankWeaponSelectGroup : MonoBehaviour
{
    [Header("Component")]
    public GameObject groupObject;
    public ToggleGroup toggleGroup;
    
    [Header("Settings")]
    public GameObject togglePrefab;
    public List<WeaponDetailsSO> weaponDetailsList;

    [Header("Debug")] 
    private bool isFirstToggle = true;
    private Toggle _currentToggle;

    private void Awake()
    {
        GenerateSelectToggle();
    }

    private void OnEnable()
    {
        // Select the first toggle by default,
        // Call the Weapon Detail Panel to update the weapon detail UI.
        if (_currentToggle == null) return;
        _currentToggle.isOn = false;
        _currentToggle.isOn = true;
    }

    private void GenerateSelectToggle()
    {
        foreach (var weaponDetails in weaponDetailsList)
        {
            var newObj = Instantiate(togglePrefab, groupObject.transform).GetComponent<TankWeaponSelectToggle>();
            newObj.GetComponent<TankWeaponSelectToggle>().Initialize(weaponDetails, this);
            var newToggle = newObj.gameObject.GetComponentInChildren<Toggle>();
            newToggle.group = toggleGroup;
            newToggle.isOn = isFirstToggle;
            _currentToggle = isFirstToggle ? newToggle : _currentToggle;
            isFirstToggle = false;
        }
    }
    
    public void ChangeToggleSelected(Toggle toggle)
    {
        _currentToggle = toggle;
    }
}
