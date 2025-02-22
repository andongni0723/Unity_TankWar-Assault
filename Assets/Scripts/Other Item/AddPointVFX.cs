using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPointVFX : MonoBehaviour
{
    [Header("Component")]
    private ParticleSystem _particleSystem;
    private ParticleSystemRenderer _particleSystemRenderer;
    
    [Header("Settings")]
    public AreaName areaName;
    public Material redMaterial;
    public Material blueMaterial;
    
    //[Header("Debug")]

    private void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _particleSystemRenderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
    }

    private void OnEnable()
    {
        EventHandler.OnAreaOccupied += OnAddPoint;
    }
    private void OnDisable()
    {
        EventHandler.OnAreaOccupied -= OnAddPoint;
    }

    private void OnAddPoint(Team team, AreaName targetAreaName)
    {
        if (areaName != targetAreaName) return;
        _particleSystemRenderer.material = team == Team.Blue ? blueMaterial : redMaterial;
        _particleSystem.Play();
    }
}
