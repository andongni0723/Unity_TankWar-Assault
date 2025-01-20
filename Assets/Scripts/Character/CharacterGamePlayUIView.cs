using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterGamePlayUIView : MonoBehaviour
{
    [Header("Component")] 
    private CharacterController _cc;
    private CharacterShoot _characterShoot;
    private Image _shootButtonImage;
    private WeaponGamePlayUI _mainWeaponUI;
    private WeaponGamePlayUI _subWeaponUI;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _characterShoot = GetComponent<CharacterShoot>();
        _shootButtonImage = GameUIManager.Instance.fireButton.GetComponent<Image>();
        _mainWeaponUI = GameUIManager.Instance.mainWeaponUI;
        _subWeaponUI = GameUIManager.Instance.subWeaponUI;
        _mainWeaponUI.weaponToggle.onValueChanged.AddListener(OnMainWeaponToggleChanged);
        _subWeaponUI.weaponToggle.onValueChanged.AddListener(OnSubWeaponToggleChanged);
        InitializeGamePlayUI();
    }

    private void Update()
    {
        UpdateImageFill();
    }
    
    private void OnMainWeaponToggleChanged(bool isOn)
    {
        if (isOn)
            _characterShoot.ChangeWeapon(TankWeaponType.MainWeapon, GameDataManager.Instance.tankMainWeaponDetails);
    }
    
    private void OnSubWeaponToggleChanged(bool isOn)
    {
        if (isOn)
            _characterShoot.ChangeWeapon(TankWeaponType.SubWeapon, GameDataManager.Instance.tankSubWeaponDetails);
    }
    
    private void UpdateImageFill()
    {
        _shootButtonImage.fillAmount = 
            _cc.mobileShootTimer.isPlay ? _cc.mobileShootTimer.currentTime / _cc.mobileShootTimer.time : 1;
    }

    private void InitializeGamePlayUI()
    {
        UpdateWeaponDetailUI(_mainWeaponUI, GameDataManager.Instance.tankMainWeaponDetails);
        UpdateWeaponDetailUI(_subWeaponUI, GameDataManager.Instance.tankSubWeaponDetails);
    }
    
    public void UpdateWeaponDetailUI(WeaponGamePlayUI weaponUI, WeaponDetailsSO weaponDetails)
    {
        weaponUI.weaponImage.sprite = weaponDetails.weaponSprite;
        weaponUI.weaponNameText.text = weaponDetails.weaponName;
        var capacity_text = weaponDetails.capacity == -1 ? "∞" : weaponDetails.capacity.ToString();
        weaponUI.weaponAmmoText.text = $"{capacity_text} / {capacity_text}";
        weaponUI.weaponImage.SetNativeSize();
    }
    
}
