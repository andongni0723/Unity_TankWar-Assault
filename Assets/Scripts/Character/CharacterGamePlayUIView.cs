using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterGamePlayUIView : NetworkBehaviour
{
    [Header("Component")] 
    private CharacterController _cc;
    private CharacterShoot _cs;
    private Image _shootButtonImage;
    private WeaponGamePlayUI _mainWeaponUI;
    private WeaponGamePlayUI _subWeaponUI;
    
    // [Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        InitializeComponent();
        InitializeGamePlayUI();
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) enabled = false;
        _mainWeaponUI.weaponToggle.onValueChanged.AddListener(OnMainWeaponToggleChanged);
        _subWeaponUI.weaponToggle.onValueChanged.AddListener(OnSubWeaponToggleChanged); 
    }

    private void Update()
    {
        UpdateImageFill();
        UpdateAmmoText();
    }
    
    private string TextOrInfinity(int value) => value == -1 ? "∞" : value.ToString();
    
    #region Initialize
    private void InitializeComponent()
    {
        _cc = GetComponent<CharacterController>();
        _cs = GetComponent<CharacterShoot>();
        _shootButtonImage = GameUIManager.Instance.fireButton.GetComponent<Image>();
        _mainWeaponUI = GameUIManager.Instance.mainWeaponUI;
        _subWeaponUI = GameUIManager.Instance.subWeaponUI;
    }

    private void InitializeGamePlayUI()
    {
        UpdateWeaponDetailUI(_mainWeaponUI, GameDataManager.Instance.tankMainWeaponDetails);
        UpdateWeaponDetailUI(_subWeaponUI, GameDataManager.Instance.tankSubWeaponDetails);
    }
    
    private void UpdateWeaponDetailUI(WeaponGamePlayUI weaponUI, WeaponDetailsSO weaponDetails)
    {
        weaponUI.weaponImage.sprite = weaponDetails.weaponSprite;
        weaponUI.weaponNameText.text = weaponDetails.weaponName;
        var capacity_text = TextOrInfinity(weaponDetails.capacity);
        weaponUI.weaponAmmoText.text = $"{capacity_text} / {capacity_text}";
        weaponUI.weaponImage.SetNativeSize();
    }
    #endregion
    
    private void OnMainWeaponToggleChanged(bool isOn)
    {
        if (isOn)
            _cs.ChangeWeapon(TankWeaponType.MainWeapon, GameDataManager.Instance.tankMainWeaponDetails);
    }
    
    private void OnSubWeaponToggleChanged(bool isOn)
    {
        if (isOn)
            _cs.ChangeWeapon(TankWeaponType.SubWeapon, GameDataManager.Instance.tankSubWeaponDetails);
    }
    
    private void UpdateImageFill()
    {
        _shootButtonImage.fillAmount = _cs.currentWeaponData.shootTimer.isPlay ? 
            _cs.currentWeaponData.shootTimer.currentTime / _cs.currentWeaponData.shootTimer.time : 1;
        
        _mainWeaponUI.weaponImageFill.fillAmount = 
            _cs.mainWeaponData.reloadTimer.isPlay && !_cs.mainWeaponData.weaponDetails.infiniteAmmo ? 
            1 - _cs.mainWeaponData.reloadTimer.currentTime / _cs.mainWeaponData.reloadTimer.time : 0;
        
        _subWeaponUI.weaponImageFill.fillAmount = 
            _cs.subWeaponData.reloadTimer.isPlay && !_cs.subWeaponData.weaponDetails.infiniteAmmo ? 
            1 - _cs.subWeaponData.reloadTimer.currentTime / _cs.subWeaponData.reloadTimer.time : 0;
    }

    private void UpdateAmmoText()
    {
        _mainWeaponUI.weaponAmmoText.text = $"{TextOrInfinity(_cs.mainWeaponData.currentAmmo)} / {TextOrInfinity(GameDataManager.Instance.tankMainWeaponDetails.capacity)}";
        _subWeaponUI.weaponAmmoText.text = $"{TextOrInfinity(_cs.subWeaponData.currentAmmo)} / {TextOrInfinity(GameDataManager.Instance.tankSubWeaponDetails.capacity)}";
    }
    
}
