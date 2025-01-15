using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDetailsSO", menuName = "ScriptableObject/EffectDetailsSO")]
public class EffectDetailsSO : ScriptableObject
{
    public string effectName;
    public string effectID;
    public Sprite effectSprite;
    [TextArea] public string effectDescription;
    public EffectType effectType;
    
    // [ShowIf("effectType", EffectType.Skill)] public SkillDetailsSO skillDetails;
    
    [ShowIf("effectType", EffectType.Status)] public Status statusDetails;
}

public enum EffectType
{
    None,
    Skill,
    Status,
}
