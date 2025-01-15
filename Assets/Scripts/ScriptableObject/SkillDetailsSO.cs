using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDetailsSO", menuName = "ScriptableObject/SkillDetailsSO")]
public class SkillDetailsSO : ScriptableObject
{
    public string skillName;
    public string skillID;
    [TextArea] public string skillDescription;
    public float skillLifeTime;
    public Sprite skillSprite;
    
    public List<SkillActionTimeline> skills;
}
