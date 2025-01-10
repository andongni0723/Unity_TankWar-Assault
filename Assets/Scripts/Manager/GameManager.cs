using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public enum Team
{
    Red,
    Blue
}

public class GameManager : MonoBehaviour
{
    [Header("Component")]
    public TMP_Text versionText;

    public GameObject errorPanel;
    public TMP_Text errorText;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        SettingGameFrameRate();
        UpdateGameVersionText();
    }

    private void OnEnable()
    {
        Application.logMessageReceived += OpenPanelShowError;
    }
    
    private void OnDisable()
    {
        Application.logMessageReceived -= OpenPanelShowError;
    }

    

    #region Initialize
    private void SettingGameFrameRate()
    {
        // #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        // Application.targetFrameRate = 300;
        // #elif UNITY_ANDROID || UNITY_IOS
        // Application.targetFrameRate = 60;
        // #endif 
    }
    private void UpdateGameVersionText() => versionText.text = $"v{Application.version}";
    #endregion
        
    private void OpenPanelShowError(string condition, string stacktrace, LogType type)
    {
        if (type is LogType.Error or LogType.Exception)
            StartCoroutine(OpenPanelAction(condition, stacktrace));
    }
    
    private IEnumerator OpenPanelAction(string errorContent, string stacktrace)
    {
        errorPanel.SetActive(true);
        errorText.text = errorContent;
        // copy the stacktrace
        GUIUtility.systemCopyBuffer = errorContent + '\n' + stacktrace;
        yield return new WaitForSeconds(3);
        errorPanel.SetActive(false);
    }
}
