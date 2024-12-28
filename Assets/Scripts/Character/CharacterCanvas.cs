using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCanvas : MonoBehaviour
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]


    private void Update()
    {
        var dir = Camera.main.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-dir);
    }
}
