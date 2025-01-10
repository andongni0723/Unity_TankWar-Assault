using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    
    [Header("Game Settings")]
    [SerializeField] public int game_fps { get; private set; }

    public override void Awake()
    {
        base.Awake();
        RestoreSettingData();
        ExecuteDataAction();
    }
    
    public void UpdateSettingData(string dataKey, object dataValue)
    {
        switch (dataKey)
        {
            case "game_fps":
                PlayerPrefs.SetInt(dataKey, (int)dataValue);
                game_fps = (int)dataValue;
                Application.targetFrameRate = game_fps;
                break;
        }
        ExecuteDataAction();
    }
    
    private void RestoreSettingData()
    {
        game_fps = PlayerPrefs.GetInt("game_fps", 60);
    }

    private void ExecuteDataAction()
    {
        Application.targetFrameRate = game_fps;
    }
    
}
