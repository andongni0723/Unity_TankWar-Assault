using System.Collections;
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
    // [Header("Debug")]
    
    private void Awake()
    {
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
    private void UpdateGameVersionText() => versionText.text = $"v{Application.version}";
    #endregion
        
    private void OpenPanelShowError(string condition, string stacktrace, LogType type)
    {
        if (type is LogType.Error or LogType.Exception && GameDataManager.Instance.isDebugMode)
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
