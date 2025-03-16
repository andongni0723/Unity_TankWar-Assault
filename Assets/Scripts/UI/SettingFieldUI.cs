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
    InputField_String,
    InputField_Int,
    InputField_Float,
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
    
    [ShowIf("fieldType", SettingFieldType.Dropdown)]
    public float startDropdownValue;
    
    [ShowIf("fieldType", SettingFieldType.Toggle)] 
    public bool startToggleValue;
    
    private bool ShouldShowInputFieldValue =>
        fieldType is SettingFieldType.InputField_String or SettingFieldType.InputField_Int or SettingFieldType.InputField_Float;
    
    private bool ShouldShowInputFieldNumberRange => 
        fieldType is SettingFieldType.InputField_Int or SettingFieldType.InputField_Float;

    
    [ShowIf("ShouldShowInputFieldValue")]
    public string startInputFieldValue;
    
    [ShowIf("ShouldShowInputFieldNumberRange")]
    [MinMaxSlider(-10, 30, true)] 
    public Vector2 inputFieldNumberRange;
    
    private void Awake()
    {
        Initialize();
        UpdateFieldValue();
    }

    private void OnEnable()
    {
        UpdateFieldValue();
    }

    private void Initialize()
    {
        switch (fieldType)
        {
            case SettingFieldType.Dropdown:
                _dropdown = GetComponentInChildren<TMP_Dropdown>();
                _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
                break;
            
            case SettingFieldType.Toggle:
                _toggle = GetComponentInChildren<Toggle>();
                _toggle.onValueChanged.AddListener(OnToggleValueChanged);
                break;
            
            case SettingFieldType.InputField_String:
            case SettingFieldType.InputField_Int:
            case SettingFieldType.InputField_Float:
                _inputField = GetComponentInChildren<TMP_InputField>();
                _inputField.onEndEdit.AddListener(OnInputFieldValueChanged);
                break;
        } 
    }

    private void UpdateFieldValue()
    {
        switch (fieldType)
        {
            case SettingFieldType.Dropdown:
                var savedOptionValue = PlayerPrefs.GetInt(settingKey.ToString(), (int)startDropdownValue);
                var matchedIndex = _dropdown.options.FindIndex(option => int.Parse(option.text) == savedOptionValue);
                _dropdown.value = matchedIndex;
                break;
            
            case SettingFieldType.Toggle:
                _toggle.isOn = PlayerPrefs.GetInt(settingKey.ToString(), startToggleValue ? 1 : 0) == 1;
                break;
            
            case SettingFieldType.InputField_String:
                _inputField.text = PlayerPrefs.GetString(settingKey.ToString(), startInputFieldValue);
                break;
            
            case SettingFieldType.InputField_Int:
                _inputField.text = PlayerPrefs.GetInt(settingKey.ToString(), int.Parse(startInputFieldValue)).ToString();
                break; 
            
            case SettingFieldType.InputField_Float:
                _inputField.text = PlayerPrefs.GetFloat(settingKey.ToString(), float.Parse(startInputFieldValue)).ToString();
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
        if (float.TryParse(newValue, out var number))
        {
            // Out of range
            if (number < inputFieldNumberRange.x)
                _inputField.text = inputFieldNumberRange.x.ToString();
            else if (number > inputFieldNumberRange.y)
                _inputField.text = inputFieldNumberRange.y.ToString();
        }
        
        if (string.IsNullOrEmpty(newValue))
            _inputField.text = startInputFieldValue;
        
        newValue = _inputField.text;
        GameDataManager.Instance.UpdateSettingData(settingKey, newValue);
    }
}
