using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SaveDataKey
{
    none,
    game_fps,
    debug_mode,
    reverse_x,
    auto_follow_enemy,
    camera_drag_speed,
}

public class GameDataManager : Singleton<GameDataManager>
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    
    [Header("Game Settings")]
    [SerializeField] public int gameFPS { get; private set; }
    [SerializeField] public bool isDebugMode { get; private set; }
    [SerializeField] public bool isReverseX { get; private set; }
    [SerializeField] public bool isAutoFollowEnemy { get; private set; }
    [SerializeField] public float cameraDragSpeed { get; private set; }

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
            case SaveDataKey.reverse_x:
                PlayerPrefs.SetInt("reverse_x", (bool)dataValue ? 1 : 0);
                isReverseX = (bool)dataValue;
                break;
            case SaveDataKey.auto_follow_enemy:
                PlayerPrefs.SetInt("auto_follow_enemy", (bool)dataValue ? 1 : 0);
                break;
            case SaveDataKey.camera_drag_speed:
                PlayerPrefs.SetFloat("camera_drag_speed", float.Parse(dataValue.ToString()));
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
        isReverseX = PlayerPrefs.GetInt("reverse_x", 0) == 1;
        isAutoFollowEnemy = PlayerPrefs.GetInt("auto_follow_enemy", 0) == 1;
        cameraDragSpeed = PlayerPrefs.GetFloat("camera_drag_speed", 0.5f);
    }
}
