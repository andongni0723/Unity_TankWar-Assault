using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Widgets;
using System.Reflection;
using Unity.Services.Multiplayer;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    [SceneName]
    public string sceneName;
    
    private bool isFirstReset = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ShutdownRoom);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        ShutdownRoom();
    }

    public void ShutdownRoom()
    {
        if(isFirstReset) return;
        isFirstReset = true;

        Debug.Log("SHUTDOWN ROOM");
        An_ConnectionManager.Instance.LeaveSession();
        NetworkManager.Singleton.Shutdown();
        LoadTargetScene();
    }
    
    public void LoadTargetScene()
    {
        SceneLoader.Instance.CallLoadScene(sceneName);
    }
}