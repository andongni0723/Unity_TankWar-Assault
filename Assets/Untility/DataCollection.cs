using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class WeaponGamePlayUI
{
    public Toggle weaponToggle;
    public Image weaponImage;
    public TMP_Text weaponNameText;
    public TMP_Text weaponAmmoText;
    public Image weaponImageFill;
}

[Serializable]
public class GameWeaponData
{
    public WeaponDetailsSO weaponDetails;
    public Timer shootTimer;
    public Timer reloadTimer;
    public int currentAmmo;
}

[Serializable]
public class ActionTimeline<T> where T : ActionTimeline<T>
{
    public float time;
    public T action;
}

[Serializable]
public class EffectActionTimeline : ActionTimeline<EffectActionTimeline>
{
    public EffectDetailsSO effectDetails;
}

[Serializable]
public class StatusActionTimeline : ActionTimeline<StatusActionTimeline>
{
    public Status statusDetails;
}

[Serializable]
public class SkillActionTimeline : ActionTimeline<SkillActionTimeline>
{
    public SkillAction skillActionDetails;
}


[Serializable]
public class Status
{
    public float duration;
    public bool isStackable;
    
    [Header("Status Action")][Space(15)]
    public StatusType statusType;
    public BuffType BuffType;
    public float value;
    public bool isRemoveOnTime;
}

public enum StatusType
{
    None,
    Buff,
    Debuff,
}

public enum BuffType
{
    None,
    Health,
    Damage,
    Speed,
    Armor,
    Invincible,
}


[Serializable]
public class SkillAction
{
    public int damage;
    public SkillActionType skillActionType;
    [ShowIf("skillActionType", SkillActionType.AOE)] public AOE aoe;
    [ShowIf("skillActionType", SkillActionType.Targeted)] public Targeted targeted;
    public List<EffectDetailsSO> onApplyEffects;
    public List<EffectDetailsSO> onEndEffects;
}

public enum SkillActionType
{
    None,
    AOE,
    Targeted,
}

[Serializable]
public class AOE 
{
    public float radius;
}

[Serializable]
public class Targeted
{
    public bool isTargetEnemy; // true = enemy, false = self
}

[System.Serializable]
public class Area
{
    // The sum of the two variables is 100
    public float blueTeamOccupiedPercentage { private set; get; }
    public float redTeamOccupiedPercentage { private set; get; }
    
    /// <summary>
    /// Sets the occupied percentage for the specified team and adjusts the other team's percentage accordingly.
    /// The total occupation will always sum to 100%.
    /// </summary>
    /// <param name="team"></param>
    /// <param name="percentage"></param>
    public void SetOccupiedPercentage(Team team, float percentage)
    {
        if(percentage is < 0 or > 100) Debug.LogError("Invalid percentage value ({percentage}). The percentage must be between 0 and 100.");
        percentage = Mathf.Clamp(percentage, 0, 100);
        
        blueTeamOccupiedPercentage = team == Team.Blue ? percentage : 100 - percentage;
        redTeamOccupiedPercentage = team == Team.Red ? percentage : 100 - percentage;
    }
}
