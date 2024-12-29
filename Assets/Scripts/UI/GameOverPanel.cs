using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    [Header("Component")]
    public GameObject winPanel;
    public GameObject losePanel;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void OnEnable()
    {
        EventHandler.OnPlayerDied += OnPlayerDied;
    }
    private void OnDisable()
    {
        EventHandler.OnPlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied(bool isSelf)
    {
        if (isSelf) losePanel.SetActive(true);
        else winPanel.SetActive(true);
    }
}
