using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSetColor : NetworkBehaviour
{
    //[Header("Component")]
    [Header("Settings")]
    public List<Renderer> ChangeColorRenderers = new();
    //[Header("Debug")]
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetColorBasedOnOwner();
    }

    /// <summary>
    /// 當物件的所有權發生變化以更改顏色時收到通知。
    /// </summary>
    /// <param name="previous">the previous owner</param>
    /// <param name="current">the current owner</param>
    protected override void OnOwnershipChanged(ulong previous, ulong current)
    {
        SetColorBasedOnOwner();
    }

    void SetColorBasedOnOwner()
    {
        // OwnerClientId 此處用於偵錯目的。現場遊戲應該使用會話管理器來確保
        // 重新連接的玩家仍然獲得相同的顏色，因為客戶端 ID 可以在
        // 斷開連接和重新連接之間重用於其他客戶端。有關會話管理器範例，請參閱 Boss Room。
        UnityEngine.Random.InitState((int) OwnerClientId);
        var color = UnityEngine.Random.ColorHSV();
        foreach (var r in ChangeColorRenderers)
        {
            r.material.color = color;
        }
    }
}
