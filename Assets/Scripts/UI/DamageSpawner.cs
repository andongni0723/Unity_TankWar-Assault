using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DamageSpawner : NetworkBehaviour
{
    public GameObject damageTextPrefab; // 拖入 DamageText Prefab
    public Canvas canvas;
    
    public void CallSpawnDamageText(int damage, Vector3 position)
    {
        if (IsServer)
            SpawnDamageTextClientRpc(damage, position);
        else
            SpawnDamageTextServerRpc(damage, position);
    }
    
    [ServerRpc]
    public void SpawnDamageTextServerRpc(int damage, Vector3 position)
    {
        SpawnDamageTextClientRpc(damage, position);
    }
    
    [ClientRpc]
    public void SpawnDamageTextClientRpc(int damage, Vector3 position)
    {
        SpawnDamageText(damage, position);
    }
    
    public void SpawnDamageText(int damage, Vector3 position)
    {
        // 從 Resources 加載 Prefab
        GameObject damageTextObj = Instantiate(damageTextPrefab, position, Quaternion.identity);

        // 設置父物件（可選）
        damageTextObj.transform.SetParent(canvas.transform, false);

        // 顯示傷害
        DamageText damageText = damageTextObj.GetComponent<DamageText>();
        damageText.ShowDamage(damage, position);
    }

}