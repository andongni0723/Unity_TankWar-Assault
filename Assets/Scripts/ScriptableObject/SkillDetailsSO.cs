using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDetailsSO", menuName = "ScriptableObject/SkillDetailsSO")]
public class SkillDetailsSO : ScriptableObject
{
    public string skillName;
    public string skillID;
    [TextArea] public string skillDescription;
    public float skillLifeTime;
    public Vector3 skillRange;
    // public GameObject skillPrefab;
    public PoolKey skillPrefabPoolKey;
    public Sprite skillSprite;
    
    public List<SkillActionTimeline> skills;
}
