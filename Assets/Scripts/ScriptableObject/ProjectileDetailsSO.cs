using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDetailsSO", menuName = "ScriptableObject/ProjectileDetailsSO")]
public class ProjectileDetailsSO : ScriptableObject
{
    public string projectileName;
    public float projectileSpeed;
    public float projectileLifeTime;
    public int projectileDamage;
    // public GameObject projectilePrefab;
    public PoolKey redPrefabPoolKey;
    public PoolKey bluePrefabPoolKey;
    

    [ToggleGroup("isOnEndAction")] public bool isOnEndAction;
    
    [ToggleGroup("isOnEndAction")] public List<EffectDetailsSO> onEndEffects;
    [ToggleGroup("isOnEndAction")] public List<SkillDetailsSO> onEndSkills;

}
