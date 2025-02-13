using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameSceneManager : Singleton<GameSceneManager>
{
    [Header("Component")]
    public Timer gameTimer;

    //[Header("Settings")]
    //[Header("Debug")]

    private void OnEnable()
    {
        EventHandler.OnGameStart += OnGameStart;
        EventHandler.OnLeaveSession += () => gameTimer.Stop();
        gameTimer.OnTimerEnd += () => EventHandler.CallOnGameEnd(NetworkManager.Singleton.IsHost);
    }

    private void OnDisable()
    {
        EventHandler.OnGameStart -= OnGameStart;
    }
    
    private void Start() => UpdateTimerUI();
    
    private void Update()
    {
        if(gameTimer.isPlay)
            UpdateTimerUI();
    }

    private void OnGameStart()
    {
        gameTimer.Stop();
        gameTimer.Play();
    }
    
    private void UpdateTimerUI()
    {
        GameUIManager.Instance.gameTimeText.text = SecondToMinuteString(gameTimer.time - gameTimer.currentTime);
    }

    private string SecondToMinuteString(float second)
    {
        var minute = (int) (second / 60);
        var sec = (int) (second % 60);
        return $"{minute:00}:{sec:00}";
    }
}
