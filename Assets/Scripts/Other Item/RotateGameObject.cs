using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
    //[Header("Component")]
    [Header("Settings")]
    public float rotateSpeed = 10f;
    //[Header("Debug")]
    
    
    // Rotate the game object Y pos with rotate speed
    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
    
}
