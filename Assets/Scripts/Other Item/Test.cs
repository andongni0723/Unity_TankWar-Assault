using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public float fixedY = 0.04f;  // 固定的 Y 坐標
    public LayerMask groundLayer; // 用來偵測滑鼠射線與地面碰撞的圖層
    
    // []public Dictionary<WeaponDetailsSO, GameObject> firePointDict = new();
    
    
    void Update()
    {
        // 取得滑鼠位置的 Ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 用來儲存射線碰撞的資訊
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // 更新球的位置
            var targetPosition = hit.point;
            targetPosition.y = fixedY; // 固定 Y 坐標
            transform.position = targetPosition;
        }
    }


    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 取得「身上(掛載腳本的物件)」的Rigidbody2D 元件
        
        rb.linearVelocity = new Vector2(1, 0); // 設定速度
        rb.angularVelocity = 1; // 設定角速度
        rb.gravityScale = 1; // 重力比例
        rb.AddForce(Vector2.right, ForceMode2D.Impulse); // 施加力量 (瞬間) J = FΔt
        rb.AddForce(Vector2.right, ForceMode2D.Force);   // 施加力量 (持續) F = ma
        rb.AddTorque(1, ForceMode2D.Impulse); // 施加扭力
    }



    private delegate void SkillAction(SkillDetailsSO data);

    private SkillAction Action;
    private void Test1()
    {
        Action += DD;
    }

    private void DD(SkillDetailsSO data)
    {
    }
}