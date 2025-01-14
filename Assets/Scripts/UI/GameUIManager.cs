using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : Singleton<GameUIManager>
{
    [Header("Component")] 
    public GameObject gamePlayUI;
    public Slider leftTrackSlider;
    public Slider rightTrackSlider;
    public Button fireButton;
    public VariableJoystick tankHeadJoystick; 
    public List<RectTransform> dragAreas;

    //[Header("Settings")]
    //[Header("Debug")]

    public override void Awake()
    {
        base.Awake();
        gamePlayUI.SetActive(false);
    }
}
