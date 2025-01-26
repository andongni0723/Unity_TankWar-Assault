using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
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
    [SerializeField] public float cameraDragSpeed { get; private set; }
    
    [Header("Tank Settings")]
    // [SerializeField] public string tankMainWeaponID { get; private set; }
    // [SerializeField] public string tankSubWeaponID { get; private set; }
    
    [SerializeField] public WeaponDetailsSO tankMainWeaponDetails { get; private set; }
    [SerializeField] public WeaponDetailsSO tankSubWeaponDetails { get; private set; }
    
    public override void Awake()
    {
        base.Awake();
        RestoreSettingData();
        RestoreTankData();
        ExecuteDataAction();
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
            case SaveDataKey.camera_drag_speed:
                PlayerPrefs.SetFloat("camera_drag_speed", float.Parse(dataValue.ToString()));
                cameraDragSpeed = float.Parse(dataValue.ToString());
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
        cameraDragSpeed = PlayerPrefs.GetFloat("camera_drag_speed", 0.5f);
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
