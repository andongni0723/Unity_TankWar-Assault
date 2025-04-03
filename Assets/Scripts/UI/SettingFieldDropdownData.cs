using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingFieldDropdownData : MonoBehaviour
{
    // [Header("Component")]
    private TMP_Dropdown _dropdown;
    
    [Header("Settings")]
    public List<string> dropdownData = new();


    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        if(dropdownData.Count != _dropdown.options.Count)
            Debug.LogError("SettingField DropdownData count not match");
    }
}
