using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ResetButton : MonoBehaviour
{
    [SceneName]
    public string sceneName;
   
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

    private void ShutdownRoom()
    {
        // 通知所有客戶端退出
        NetworkManager.Singleton.Shutdown();

        // 可選：返回主菜單場景
        LoadTargetScene();
    }
    
    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}