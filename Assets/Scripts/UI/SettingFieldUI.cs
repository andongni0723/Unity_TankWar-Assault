using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum SettingFieldType
{
    None,
    Dropdown,
    Toggle,
    InputField
}

public class SettingFieldUI : MonoBehaviour
{
    [Header("Component")]
    private TMP_Dropdown _dropdown;
    private Toggle _toggle;
    private TMP_InputField _inputField;
    
    [Header("Settings")]
    public SettingFieldType fieldType;
    public SaveDataKey settingKey;
    
    [ShowIf("fieldType", SettingFieldType.Dropdown)] public float startDropdownValue;
    [ShowIf("fieldType", SettingFieldType.Toggle)] public bool startToggleValue;
    [ShowIf("fieldType", SettingFieldType.InputField)] public string startInputFieldValue;
    
    private void Awake()
    {
        switch (fieldType)
        {
            case SettingFieldType.Dropdown:
                _dropdown = GetComponentInChildren<TMP_Dropdown>();
                _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
                // value data to key
                var savedOptionValue = PlayerPrefs.GetInt(settingKey.ToString(), (int)startDropdownValue);
                var matchedIndex = _dropdown.options.FindIndex(option => int.Parse(option.text) == savedOptionValue);
                _dropdown.value = matchedIndex;
                break;
            
            case SettingFieldType.Toggle:
                _toggle = GetComponentInChildren<Toggle>();
                _toggle.onValueChanged.AddListener(OnToggleValueChanged);
                _toggle.isOn = PlayerPrefs.GetInt(settingKey.ToString(), startToggleValue ? 1 : 0) == 1;
                break;
            
            case SettingFieldType.InputField:
                _inputField = GetComponentInChildren<TMP_InputField>();
                _inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                _inputField.text = PlayerPrefs.GetString(settingKey.ToString(), startInputFieldValue);
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
    
    private void OnInputFieldValueChanged(string newValue)
    {
        GameDataManager.Instance.UpdateSettingData(settingKey, newValue);
    }
}
