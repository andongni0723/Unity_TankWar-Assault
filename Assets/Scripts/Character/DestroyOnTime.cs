using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    //[Header("Component")]
    [Header("Settings")]
    public float time = 3;
    //[Header("Debug")]
    
    private void Awake()
    {
        Destroy(gameObject, time);
    }
}
