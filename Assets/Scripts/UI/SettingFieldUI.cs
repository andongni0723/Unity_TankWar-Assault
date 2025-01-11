using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum SettingFieldType
{
    None,
    Dropdown,
    Toggle
}

public class SettingFieldUI : MonoBehaviour
{
    [Header("Component")]
    private TMP_Dropdown _dropdown;
    private Toggle _toggle;
    
    [Header("Settings")]
    public SettingFieldType fieldType;
    public SaveDataKey settingKey;
    //[Header("Debug")]

    private void Awake()
    {
        switch (fieldType)
        {
            case SettingFieldType.Dropdown:
                _dropdown = GetComponentInChildren<TMP_Dropdown>();
                _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
                // value data to key
                var savedOptionValue = PlayerPrefs.GetInt(settingKey.ToString(), 60);
                var matchedIndex = _dropdown.options.FindIndex(option => int.Parse(option.text) == savedOptionValue);
                _dropdown.value = matchedIndex;
                break;
            case SettingFieldType.Toggle:
                _toggle = GetComponentInChildren<Toggle>();
                _toggle.onValueChanged.AddListener(OnToggleValueChanged);
                _toggle.isOn = PlayerPrefs.GetInt(settingKey.ToString(), 1) == 1;
                break;
        }
    }

    private void OnDropdownValueChanged(int newValue)
    {
        GameDataManager.Instance.UpdateSettingData(settingKey, int.Parse(_dropdown.options[newValue].text));
    }
    
    private void OnToggleValueChanged(bool newValue)
    {
        GameDataManager.Instance.UpdateSettingData(settingKey, newValue);
    }
}
