using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankWeaponSelectToggle : MonoBehaviour
{
    [Header("Component")] 
    private Toggle _toggle;
    private TMP_Text _weaponNameText;
    private TankWeaponSelectGroup _group;
    
    [Header("Settings")]
    private WeaponDetailsSO _weaponDetails;
    
    //[Header("Debug")]
    
    private void Awake()
    {
        _weaponNameText = GetComponentInChildren<TMP_Text>();
        _toggle = GetComponentInChildren<Toggle>();
    }
    
    public void Initialize(WeaponDetailsSO data, TankWeaponSelectGroup group)
    {
        _weaponDetails = data;
        _group = group;
        _weaponNameText.text = _weaponDetails.weaponName;
        _toggle.onValueChanged.AddListener(CallWeaponSelectToggleSelected);
    }
    
    private void CallWeaponSelectToggleSelected(bool isOn)
    {
        if (!isOn) return;
        EventHandler.CallOnWeaponSelectToggleSelected(_weaponDetails);
        _group.ChangeToggleSelected(_toggle);
    }
}
