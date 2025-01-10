using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SaveDataKey
{
    none,
    game_fps,
    debug_mode
}

public class GameDataManager : Singleton<GameDataManager>
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    
    [Header("Game Settings")]
    [SerializeField] public int gameFPS { get; private set; }
    [SerializeField] public bool isDebugMode { get; private set; }

    public override void Awake()
    {
        base.Awake();
        RestoreSettingData();
        ExecuteDataAction();
    }
    
    public void UpdateSettingData(SaveDataKey dataKey, object dataValue)
    {
        switch (dataKey)
        {
            case SaveDataKey.game_fps:
                PlayerPrefs.SetInt("game_fps", (int)dataValue);
                gameFPS = (int)dataValue;
                break;
            case SaveDataKey.debug_mode:
                PlayerPrefs.SetInt("debug_mode", (bool)dataValue ? 1 : 0);
                isDebugMode = (bool)dataValue;
                break;
        }
        ExecuteDataAction();
    }
    private void ExecuteDataAction()
    {
        Application.targetFrameRate = gameFPS;
    }
    
    private void RestoreSettingData()
    {
        gameFPS = PlayerPrefs.GetInt("game_fps", 60);
        isDebugMode = PlayerPrefs.GetInt("debug_mode", 1) == 1;
    }
}
