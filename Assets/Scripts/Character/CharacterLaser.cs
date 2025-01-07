using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable Unity.InefficientPropertyAccess

public class CharacterLaser : NetworkBehaviour
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    public float laserMaxDistance = 100f;  
    public float laserWidth = 0.1f;  
    public LineRenderer lineRenderer; 
    public LayerMask collisionLayer;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) return;
        
        // Don't display enemy laser
        lineRenderer.enabled = false;
        enabled = false;
    }

    void Start()
    {
        lineRenderer ??= GetComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
    }

    void Update()
    {
        FireLaser();
    }

    void FireLaser()
    {
        var laserDirection = lineRenderer.transform.up;
        var laserStart = lineRenderer.transform.position;
        var laserEnd = laserStart + laserDirection * laserMaxDistance;
        
        if (Physics.Raycast(laserStart, laserDirection, out var hit, laserMaxDistance, collisionLayer))
            laserEnd = hit.point;

        lineRenderer.SetPosition(0, laserStart);
        lineRenderer.SetPosition(1, laserEnd);
    }
}
