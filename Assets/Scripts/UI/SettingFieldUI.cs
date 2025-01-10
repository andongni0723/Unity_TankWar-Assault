using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingFieldUI : MonoBehaviour
{
    [Header("Component")]
    private TMP_Dropdown _dropdown;
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        _dropdown = GetComponentInChildren<TMP_Dropdown>();
    }
    
    public void OnDropdownValueChanged()
    {
        Debug.Log(GameDataManager.Instance);
        Debug.Log(_dropdown.value);
        GameDataManager.Instance.UpdateSettingData("game_fps", int.Parse(_dropdown.options[_dropdown.value].text));
    }
}
