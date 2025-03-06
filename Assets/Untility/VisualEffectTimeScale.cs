using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
[ExecuteInEditMode]
public class VisualEffectTimeScale : MonoBehaviour
{
     [Range(0.0f, 10.0f)] public float simulationTimeScale = 1.0f;

    private VisualEffect _vfx;

    private void OnValidate()
    {
        _vfx = gameObject.GetComponent<VisualEffect>();
        _vfx.playRate = simulationTimeScale;
    }

    private void Awake()
    {
        _vfx = gameObject.GetComponent<VisualEffect>();
        _vfx.playRate = simulationTimeScale;
    }
}