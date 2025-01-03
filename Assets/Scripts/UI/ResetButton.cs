using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Multiplayer;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Multiplayer.Widgets;

public class ResetButton : MonoBehaviour
{
    [SceneName]
    public string sceneName;

    private bool isFirstReset = false;
   
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
        NetworkManager.Singleton.Shutdown();
        LoadTargetScene();
    }
    
    
    
    public void LoadTargetScene()
    {
        SceneLoader.Instance.CallLoadScene(sceneName);
    }
}