using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public float fixedY = 0.04f;  // 固定的 Y 坐標
    public LayerMask groundLayer; // 用來偵測滑鼠射線與地面碰撞的圖層
    
    void Update()
    {
        // 取得滑鼠位置的 Ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 用來儲存射線碰撞的資訊
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // 更新球的位置
            Vector3 targetPosition = hit.point;
            targetPosition.y = fixedY; // 固定 Y 坐標
            transform.position = targetPosition;
        }
    }
}
