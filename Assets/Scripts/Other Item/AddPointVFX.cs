using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AddPointVFX : MonoBehaviour
{
    [Header("Component")]
    private ParticleSystem _particleSystem;
    private ParticleSystemRenderer _particleSystemRenderer;
    
    [Header("Settings")]
    public AreaName areaName;
    // public Material redMaterial;
    // public Material blueMaterial;
    public VisualEffect redVFX;
    public VisualEffect blueVFX;
    
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
        // _particleSystemRenderer.material = team == Team.Blue ? blueMaterial : redMaterial;
        // _particleSystemRenderer.material = team == Team.Blue ? blueVFX.material : redVFX.material;
        if (team == Team.Blue)
        {
            blueVFX.gameObject.SetActive(true);
            blueVFX.Play();
        }
        else
        {
            redVFX.gameObject.SetActive(true);
            redVFX.Play();
        }
    }
}
