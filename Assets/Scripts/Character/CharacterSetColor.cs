using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;


public class CharacterSetColor : NetworkBehaviour
{
    [Header("Component")]
    private CharacterController _cc;
    
    [Header("Settings")]
    public List<Renderer> ChangeColorRenderers = new();
    public List<Renderer> machineGunChangeColorRenderers = new();
    //[Header("Debug")]
    
    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }
    
    public void SetColorBasedOnOwner()
    {
        var color = _cc.team.Value == Team.Blue ? Color.blue : Color.red;
        // 更新所有渲染器的顏色
        foreach (var r in ChangeColorRenderers)
        {
            r.material.color = color;
        }
        
        foreach (var r in machineGunChangeColorRenderers)
        {
            r.material.color = color;
        }
    }
}
