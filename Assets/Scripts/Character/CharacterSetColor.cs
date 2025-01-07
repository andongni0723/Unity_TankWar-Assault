using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class CharacterSetColor : NetworkBehaviour
{
    [Header("Component")]
    private CharacterController _cc;
    
    [Header("Settings")]
    public List<Renderer> ChangeColorRenderers = new();
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
    }
}
