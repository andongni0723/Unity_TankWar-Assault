using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[Header("Component")]
    private Rigidbody _rb;
    
    [Header("Settings")]
    public float speed = 10;
    //[Header("Debug")]

    private void Awake()
    {
        InitialComponent();
        Destroy(gameObject, 3);
    }
    

    private void InitialComponent()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.up * speed;
    }
}
