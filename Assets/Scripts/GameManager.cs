using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum Team
{
    Red,
    Blue
}

public class GameManager : MonoBehaviour
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        Application.targetFrameRate = 300;
    }
}
