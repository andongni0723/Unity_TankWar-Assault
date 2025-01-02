using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ButtonFunction
{
    StartGame,
    QuitGame,
    OpenPanel,
}
public class MenuButton : MonoBehaviour
{
    [Header("Component")]
    private Button _button;
    
    [Header("Settings")]
    public ButtonFunction buttonFunction;
    
    [Header("Open Panel Settings")]
    [ShowIf("buttonFunction", ButtonFunction.OpenPanel)]
    public UIAnimation targetPanel;
    [ShowIf("buttonFunction", ButtonFunction.OpenPanel)]
    public UIAnimation closePanel;
    [ShowIf("buttonFunction", ButtonFunction.OpenPanel)]
    public UnityEvent AfterOpenPanel;
    
    //[Header("Debug")]
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        switch (buttonFunction)
        {
            case ButtonFunction.StartGame:
                SceneManager.LoadSceneAsync("GameScene");
                break;
            case ButtonFunction.QuitGame:
                Application.Quit();
                break;
            case ButtonFunction.OpenPanel:
                PanelManager.Instance.ChangePanel(closePanel, targetPanel);
                AfterOpenPanel?.Invoke();
                break;
        }
    }
}
