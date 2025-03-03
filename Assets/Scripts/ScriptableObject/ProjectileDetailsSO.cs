using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ProjectileDetailsSO", menuName = "ScriptableObject/ProjectileDetailsSO")]
public class ProjectileDetailsSO : ScriptableObject
{
    public string projectileName;
    public WeaponFireType fireType;
    [ShowIf("fireType", WeaponFireType.AOE)] public float hitTimerDelay;
    public float projectileSpeed;
    public float projectileLifeTime;
    public int projectileDamage;
    public float projectileHurtRadius; 
    public PoolKey redPrefabPoolKey;
    public PoolKey bluePrefabPoolKey;
    

    [ToggleGroup("isOnEndAction")] public bool isOnEndAction;
    
    [ToggleGroup("isOnEndAction")] public List<EffectDetailsSO> onEndEffects;
    [ToggleGroup("isOnEndAction")] public List<SkillDetailsSO> onEndSkills;

}
