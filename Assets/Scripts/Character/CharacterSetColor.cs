using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class CharacterSetColor : NetworkBehaviour
{
    //[Header("Component")]
    private CharacterController _cc;
    
    [Header("Settings")]
    public List<Renderer> ChangeColorRenderers = new();
    //[Header("Debug")]


    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    // public override void OnNetworkSpawn()
    // {
    //     if(IsOwner) 
    //         SetColorBasedOnOwner();
    // }
    //
    // /// <summary>
    // /// 當物件的所有權發生變化以更改顏色時收到通知。
    // /// </summary>
    // /// <param name="previous">the previous owner</param>
    // /// <param name="current">the current owner</param>
    // protected override void OnOwnershipChanged(ulong previous, ulong current)
    // {
    //     // SetColorBasedOnOwner();
    //     if(IsOwner)
    //         SetColorBasedOnOwner();
    // }

    public void SetColorBasedOnOwner()
    {
        // OwnerClientId 此處用於偵錯目的。現場遊戲應該使用會話管理器來確保
        // 重新連接的玩家仍然獲得相同的顏色，因為客戶端 ID 可以在
        // 斷開連接和重新連接之間重用於其他客戶端。有關會話管理器範例，請參閱 Boss Room。
        // Random.InitState((int) OwnerClientId);
        
        // 隨機選擇紅色或藍色
        // var isRed = Random.value > 0.5f; // 50% 機率選擇紅或藍
        // var color = isRed ? Color.red : Color.blue;
        // var color = Random.ColorHSV();
        // var color = IsHost ? Color.blue : Color.red;
        var color = _cc.team.Value == Team.Blue ? Color.blue : Color.red;
        
        // 更新所有渲染器的顏色
        foreach (var r in ChangeColorRenderers)
        {
            r.material.color = color;
        }
        
        
        // var color = IsOwner ? Color.blue : Color.red;
        // foreach (var r in ChangeColorRenderers)
        // {
        //     r.material.color = color;
        // }
    }
}
