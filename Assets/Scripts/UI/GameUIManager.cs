using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUIManager : Singleton<GameUIManager>
{
    [Header("Component")] 
    [Header("GamePlay UI")]
    public GameObject gamePlayUI;
    public CanvasGroup gamePlayCanvasGroup;
    public Slider leftTrackSlider;
    public Slider rightTrackSlider;
    public Button fireButton;
    public Button cancelFireButton;
    public VariableJoystick tankHeadJoystick; 
    public List<RectTransform> dragAreas;
    public TMP_Text gameTimeText;

    [Space(15)] 
    public CanvasGroup PlayerDiedPanelCanvasGroup;
    public RespawnTimer RespawnTimerObject;
    
    [Space(15)]
    public WeaponGamePlayUI mainWeaponUI;
    public WeaponGamePlayUI subWeaponUI;

    [Space(15)] 
    public GameObject winPanel;
    public GameObject losePanel;

    //[Header("Settings")]
    //[Header("Debug")]

    public override void Awake()
    {
        base.Awake();
        gamePlayUI.SetActive(false);
        PlayerDiedPanelCanvasGroup.gameObject.SetActive(false);
        gamePlayCanvasGroup.alpha = GameDataManager.Instance.gameplayUIAlpha;
    }

    private void OnEnable()
    {
        EventHandler.OnPlayerDied += OnPlayerDied;
        EventHandler.OnPlayerRespawn += OnPlayerRespawn;
    }

    private void OnDisable()
    {
        EventHandler.OnPlayerDied -= OnPlayerDied;
        EventHandler.OnPlayerRespawn -= OnPlayerRespawn;
    }
    
    private void OnPlayerDied(bool isOwner, Timer respawnTimer)
    {
        if(!isOwner) return;
        gamePlayUI.SetActive(false);
        PlayerDiedPanelCanvasGroup.alpha = 0;
        PlayerDiedPanelCanvasGroup.gameObject.SetActive(true);
        PlayerDiedPanelCanvasGroup.DOFade(1, 0.3f);
        RespawnTimerObject.CallPlayRespawnAnimation(respawnTimer);
    }
    
    private void OnPlayerRespawn(bool isOwner)
    {
        if(!isOwner) return;
        gamePlayUI.SetActive(true);
        PlayerDiedPanelCanvasGroup.DOFade(0, 0.3f).OnComplete(() =>
            PlayerDiedPanelCanvasGroup.gameObject.SetActive(false));
    }
}
