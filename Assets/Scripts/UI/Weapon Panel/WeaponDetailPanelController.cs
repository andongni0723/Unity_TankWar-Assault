using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDetailPanelController : MonoBehaviour
{
    [Header("Component")]
    private WeaponDetailPanelView _view;
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        _view = GetComponent<WeaponDetailPanelView>();
    }

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
        _view.UpdateWeaponDetailUI(data);
    }
}
