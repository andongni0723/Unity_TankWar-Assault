using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisconnectPanel : MonoBehaviour
{
    [Header("Component")]
    public GameObject disconnectPanel;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong obj)
    {
        disconnectPanel.SetActive(true);
    }
}
