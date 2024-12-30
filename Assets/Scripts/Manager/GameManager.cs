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
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        Application.targetFrameRate = 300;
#elif UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = 120;
#endif
    }
}
