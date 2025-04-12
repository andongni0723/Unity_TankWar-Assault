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
    drag_camera,
    stop_button_expand,
    stop_button_expand_time,
    gameplay_ui_alpha,
    tank_move_operation,
}

public enum TankWeaponType
{
    None,
    MainWeapon,
    SubWeapon,
}

public class GameDataManager : Singleton<GameDataManager>
{
    //[Header("Component")]
    [Header("Settings")]
    public Dictionary<string, WeaponDetailsSO> weaponDetailsDict = new();
    //[Header("Debug")]
    
    [Header("Game Settings")]
    [SerializeField] public int gameFPS { get; private set; }
    [SerializeField] public bool isDebugMode { get; private set; }
    [SerializeField] public bool isReverseX { get; private set; }
    [SerializeField] public bool isAutoFollowEnemy { get; private set; }
    [SerializeField] public bool canDragCamera { get; private set; }
    [SerializeField] public float cameraDragSpeed { get; private set; }
    [SerializeField] public bool stopButtonEffectExpand { get; private set; }
    [SerializeField] public float startButtonExpandTime { get; private set; }
    [SerializeField] public float gameplayUIAlpha { get; private set; }
    
    [SerializeField] public TankMoveOperation tankMoveOperation { get; private set; }

    [Header("Tank Settings")]
    [SerializeField] public WeaponDetailsSO tankMainWeaponDetails { get; private set; }
    [SerializeField] public WeaponDetailsSO tankSubWeaponDetails { get; private set; }
    
    public override void Awake()
    {
        base.Awake();
        RestoreSettingData();
        RestoreTankData();
        ExecuteDataAction();
        Debug.Log(SaveDataKey.debug_mode.ToString());
    }

    #region Data Action
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
                isAutoFollowEnemy = (bool)dataValue;
                break;
            case SaveDataKey.drag_camera:
                PlayerPrefs.SetInt("drag_camera", (bool)dataValue ? 1 : 0);
                canDragCamera = (bool)dataValue;
                break;
            case SaveDataKey.camera_drag_speed:
                PlayerPrefs.SetFloat("camera_drag_speed", float.Parse(dataValue.ToString()));
                cameraDragSpeed = float.Parse(dataValue.ToString());
                break;
            case SaveDataKey.stop_button_expand:
                PlayerPrefs.SetInt("stop_button_expand", (bool)dataValue ? 1 : 0);
                stopButtonEffectExpand = (bool)dataValue;
                break;
            case SaveDataKey.stop_button_expand_time:
                PlayerPrefs.SetFloat("stop_button_expand_time", float.Parse(dataValue.ToString()));
                startButtonExpandTime = float.Parse(dataValue.ToString());
                break;
            case SaveDataKey.gameplay_ui_alpha:
                PlayerPrefs.SetFloat("gameplay_ui_alpha", float.Parse(dataValue.ToString()));
                gameplayUIAlpha = float.Parse(dataValue.ToString());
                break;
            case SaveDataKey.tank_move_operation:
                PlayerPrefs.SetInt("tank_move_operation", (int)dataValue);
                tankMoveOperation = (TankMoveOperation)dataValue;
                break;
        }

        ExecuteDataAction();
    }

    public void UpdateTankData(TankWeaponType dataKey, WeaponDetailsSO data)
    {
        switch (dataKey)
        {
            case TankWeaponType.MainWeapon:
                tankMainWeaponDetails = data;
                PlayerPrefs.SetString("tank_main_weapon_id", data.weaponID);
                break;
            case TankWeaponType.SubWeapon: 
                tankSubWeaponDetails = data;
                PlayerPrefs.SetString("tank_sub_weapon_id", data.weaponID);
                break;
        }
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
        stopButtonEffectExpand = PlayerPrefs.GetInt("stop_button_expand", 0) == 1;
        cameraDragSpeed = PlayerPrefs.GetFloat("camera_drag_speed", 0.05f);
        canDragCamera = PlayerPrefs.GetInt("drag_camera", 1) == 1;
        startButtonExpandTime = PlayerPrefs.GetFloat("stop_button_expand_time", 0.3f);
        gameplayUIAlpha = PlayerPrefs.GetFloat("gameplay_ui_alpha", 1f);
        tankMoveOperation = (TankMoveOperation)PlayerPrefs.GetInt("tank_move_operation", 1);
    }
    
    private void RestoreTankData()
    {
        tankMainWeaponDetails = UseWeaponIDGetWeaponDetails(PlayerPrefs.GetString("tank_main_weapon_id", "WM001"));
        tankSubWeaponDetails = UseWeaponIDGetWeaponDetails(PlayerPrefs.GetString("tank_sub_weapon_id", "WS001"));
    }
    #endregion
    
    /// <summary>
    /// Use weapon ID to get weapon details
    /// </summary>
    /// <param name="weaponID"></param>
    /// <returns></returns>
    public WeaponDetailsSO UseWeaponIDGetWeaponDetails(string weaponID)
    {
        if (weaponDetailsDict.TryGetValue(weaponID, out var data)) return data;
        // Not found
        Debug.LogError("Weapon ID not found: " + weaponID);
        return null;
    }
}
