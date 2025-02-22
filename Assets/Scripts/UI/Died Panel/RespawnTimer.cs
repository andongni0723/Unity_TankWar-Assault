using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RespawnTimer : MonoBehaviour
{
    [Header("Component")]
    public Image timerImage;
    public TMP_Text timerText;
    //[Header("Settings")]
    
    [Header("Debug")]
    private bool _isRespawn;
    private Timer _respawnTimer;

    private void Update()
    {
        if(_isRespawn)
            UpdateUI(_respawnTimer);
    }

    /// <summary>
    /// Call By GameUIManager.cs
    /// </summary>
    /// <param name="respawnTimer"></param>
    public void CallPlayRespawnAnimation(Timer respawnTimer)
    {
        _isRespawn = true;
        _respawnTimer = respawnTimer;
    }
    
    private void UpdateUI(Timer respawnTimer)
    {
        timerImage.fillAmount = 1 - respawnTimer.currentTime % 1;
        timerText.text = ((int)respawnTimer.time - (int)respawnTimer.currentTime).ToString("0");
        
        // When Timer End
        if (respawnTimer.isPlay) return;
        _isRespawn = false;
        timerImage.fillAmount = 0;
        timerText.text = "0";
    }
}
