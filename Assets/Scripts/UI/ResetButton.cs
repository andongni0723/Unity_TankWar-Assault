using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ResetButton : MonoBehaviour
{
    // public void OnResetButtonClicked()
    // {
    //     if (NetworkManager.Singleton.IsServer)
    //     {
    //         // Server 處理場景重置
    //         QuitRoomAndResetScene();
    //     }
    //     else
    //     {
    //         // Client 發送請求給 Server
    //         QuitRoomAndResetSceneServerRpc();
    //     }
    // }
    //
    // [ServerRpc(RequireOwnership = false)]
    // private void QuitRoomAndResetSceneServerRpc()
    // {
    //     QuitRoomAndResetScene();
    // }
    //
    // public void LoadCurrentScene()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }
    //
    // private void QuitRoomAndResetScene()
    // {
    //     try {
    //         if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
    //         {
    //             Debug.LogWarning("NetworkManager not in expected state");
    //             SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //             return;
    //         }
    //
    //         if (NetworkManager.Singleton.IsServer)
    //         {
    //             List<ulong> clientIds = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
    //             foreach (var clientId in clientIds)
    //             {
    //                 try {
    //                     NetworkManager.Singleton.DisconnectClient(clientId);
    //                 }
    //                 catch (System.Exception e)
    //                 {
    //                     Debug.LogError($"Error disconnecting client {clientId}: {e}");
    //                 }
    //             }
    //         }
    //
    //         NetworkManager.Singleton.Shutdown();
    //         StartCoroutine(ResetSceneWithDelay(1f));
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogError($"Error in QuitRoomAndResetScene: {e}");
    //         // 發生錯誤時的備用處理
    //         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //     }
    // }
    //
    // private IEnumerator ResetSceneWithDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //
    //     // 取得當前場景名稱
    //     string currentSceneName = SceneManager.GetActiveScene().name;
    //
    //     // Server 重置場景
    //     if (NetworkManager.Singleton.IsServer)
    //     {
    //         NetworkManager.Singleton.SceneManager.LoadScene(currentSceneName, LoadSceneMode.Single);
    //     }
    //     else
    //     {
    //         // Client 單獨重新加載場景
    //         SceneManager.LoadScene(currentSceneName);
    //     }
    // }
}