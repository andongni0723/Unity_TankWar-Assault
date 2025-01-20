using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
    
    [Space(15)]
    public WeaponGamePlayUI mainWeaponUI;
    public WeaponGamePlayUI subWeaponUI;

    //[Header("Settings")]
    //[Header("Debug")]

    public override void Awake()
    {
        base.Awake();
        gamePlayUI.SetActive(false);
    }
}
