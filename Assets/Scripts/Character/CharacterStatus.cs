using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class StatusEffect
{
    public SkillDetailsSO statusFrom;
    public EffectDetailsSO data;
    public int count;

    public StatusEffect(SkillDetailsSO statusFrom, EffectDetailsSO data, int count)
    {
        this.statusFrom = statusFrom;
        this.data = data;
        this.count = count;
    }
}

public class CharacterStatus : MonoBehaviour
{
    [Header("Component")]
    private CharacterController _cc;
    
    // [Header("Settings")]
    [Header("Debug")]
    [ReadOnly][SerializeField]private List<StatusEffect> _effectList = new();
    
    [Header("Event")] 
    public Action<StatusEffect, int, int> EffectChange;

    private void CallEffectChange(StatusEffect statusEffect, int oldValue, int newValue) =>
        EffectChange?.Invoke(statusEffect, oldValue, newValue);
    
    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    // private void OnEnable()
    // {
    //     EventHandler.ChangeStateDone += OnChangeStateDone;
    // }
    //
    // private void OnDisable()
    // {
    //     EventHandler.ChangeStateDone -= OnChangeStateDone;
    // }
    
    // private void OnChangeStateDone(GameState newState) => OnStateStart(character, newState);

    // ----------------- Tools -----------------
    public void SetStatusList(List<StatusEffect> effectList)
    {
        // this.statusList.ForEach(s => s.data.script.OnExit(character, s.statusFrom, s.data));
        _effectList.ForEach(s =>
        {
            OnExit(_cc, s.statusFrom, s.data);
            CallEffectChange(s, s.count, 0);
        });
        _effectList = new(effectList); 
        _effectList.ForEach(s =>
        {
            OnEnter(_cc, s.statusFrom, s.data);
            CallEffectChange(s, 0, s.count);
        });
    }
    public List<StatusEffect> GetStatusList() => _effectList;
    
    /// <summary>
    /// Adds a status effect to the character. If the status effect already exists, increments the count.
    /// </summary>
    /// <param name="skillDetails">The skill that applies this status effect.</param>
    /// <param name="effectDetails">The status effect data.</param>
    /// <param name="count">The initial count of the status effect.</param>
    public void AddStatusEffect(SkillDetailsSO skillDetails, EffectDetailsSO effectDetails, int count)
    {
        var status = _effectList.FirstOrDefault(x => x.data == effectDetails);
        if (status != null)
        {
            if (!effectDetails.statusDetails.isStackable && status.count == 1) return;
            
            CallEffectChange(status, status.count, status.count + count);
            status.count += count;
        }
        else
        {
            var newStatus = new StatusEffect(skillDetails, effectDetails, count);
            _effectList.Add(newStatus);
            OnEnter(_cc, skillDetails, effectDetails); // 只有在新的狀態效果時才調用
            CallEffectChange(newStatus, 0, count);
        }
    }
    
    /// <summary>
    /// Removes a status effect from the character. 
    /// Decreases the count of the specified status effect, and if the count reaches zero, the status effect is completely removed.
    /// Calls the OnExit method when the status effect is removed.
    /// </summary>
    /// <param name="skillDetails">The skill that applied the status effect.</param>
    /// <param name="statusEffect">The status effect to be removed.</param>
    /// <param name="removeCount">The number of times the status effect should be decremented.</param>
    public void RemoveStatusEffect(SkillDetailsSO skillDetails, EffectDetailsSO statusEffect, int removeCount)
    {
        var effect = _effectList.FirstOrDefault(x => x.data == statusEffect);
        if (effect == null) return;
    
        var newCount = Math.Max(effect.count - removeCount, 0);
        CallEffectChange(effect, effect.count, newCount);
        effect.count = newCount;
        
        if (effect.count > 0) return;
        // Effect is removed
        _effectList.Remove(effect);
        OnExit(_cc, skillDetails, statusEffect); // 只有當狀態效果完全移除時才調用
    }
    /// <summary>
    /// Removes a status effect from the character.  
    /// </summary>
    /// <param name="skillDetails"></param>
    /// <param name="statusEffect"></param>
    public void RemoveStatusEffect(SkillDetailsSO skillDetails, EffectDetailsSO statusEffect)
    {
        RemoveStatusEffect(skillDetails, statusEffect, GetStatusCount(statusEffect));
    }

    public int GetStatusCount(EffectDetailsSO statusEffect)
    {
        return !_effectList.Exists(x => x.data == statusEffect) ? 0 : // not found 
            _effectList.Find(x => x.data == statusEffect).count;
    }
    
    // private MultipleGameState ConvertToMultipleGameState(GameState gameState)
    // {
    //     return (MultipleGameState)(1 << (int)gameState);
    // }
    
    // ----------------- Status -----------------

    public void OnEnter(CharacterController character, SkillDetailsSO skillDetails, EffectDetailsSO effectDetails)
    {
        // switch (effectDetails.StatusType)
        // {
        //     case StatusType.None:
        //         break;
        //     case StatusType.SkillChange:
        //         this.character.characterSkillButtonsGroup
        //             .UseSkillIDToSkillButton(skillDetails.skillID)
        //             .OnEnter(character, skillDetails, effectDetails);
        //         // EventHandler.CallCharacterAddStatusEffect(character, skillDetails, statusDetails);
        //         break;
        //     
        //     default:
        //         Debug.LogWarning($"Unknown StatusType: {effectDetails.StatusType}");
        //         break;
        //
        // }
    }
    
    // public void OnStateStart(Character character, GameState newState)
    // {
    //     var currentMultGameState = ConvertToMultipleGameState(newState);
    //
    //     for (int i = 0; i < statusList.Count; i++)
    //     {
    //         switch (statusList[i].data.disappearType)
    //         {
    //             case StatusEffectDisappearType.None:
    //                 break;
    //             
    //             case StatusEffectDisappearType.Event:
    //                 if ((statusList[i].data.DisappearWhenGameState & currentMultGameState) != 0)
    //                 {
    //                     RemoveStatusEffect(statusList[i].statusFrom, statusList[i].data, 1);
    //                 }
    //                 break;
    //             
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         } 
    //     }
    // }

    // public void OnStateEnd(CharacterController character, GameState cur)
    // {
    //     throw new System.NotImplementedException();
    // }

    public void OnExit(CharacterController character, SkillDetailsSO skillDetails, EffectDetailsSO effectDetails)
    {
        // switch (statusDetails.StatusType)
        // {
        //     case StatusType.None:
        //         break;
        //     case StatusType.SkillChange:
        //         this.character.characterSkillButtonsGroup
        //             .UseSkillIDToSkillButton(skillDetails.skillID)
        //             .OnExit(character, skillDetails, statusDetails);
        //         break;
        //     
        //     default:
        //         throw new ArgumentOutOfRangeException();
        // }
    }
}
