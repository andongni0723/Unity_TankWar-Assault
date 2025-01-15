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
    None,
    StartGame,
    LeaveRoom,
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
    public UnityEvent BeforeOpenPanel;
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
                _button.interactable = false;
                SceneLoader.Instance.CallLoadScene("GameScene");
                break;
            case ButtonFunction.LeaveRoom:
                _button.interactable = false;
                SceneLoader.Instance.CallLoadScene("StartScene");
                break;
            case ButtonFunction.QuitGame:
                Application.Quit();
                break;
            case ButtonFunction.OpenPanel:
                BeforeOpenPanel?.Invoke();
                PanelManager.Instance.ChangePanel(closePanel, targetPanel);
                AfterOpenPanel?.Invoke();
                break;
        }
    }
}
