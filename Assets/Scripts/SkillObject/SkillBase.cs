using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : PoolableObject
{
    //[Header("Component")]
    [Header("Settings")]
    [SerializeField] protected SkillDetailsSO _skillDetails;
    public Timer destroyTimer;
    public Timer skillTimer;

    [Header("Debug")] 
    protected float _currentTime;
    // protected SkillAction _currentSkillAction;
    public bool isDrawAoeRadius;
    
    protected virtual void Awake()
    {
        destroyTimer.OnTimerEnd += ReturnToPoolAction;
    }

    private void OnDrawGizmos()
    {
        if(isDrawAoeRadius &&
           _skillDetails != null && 
           _skillDetails.skills.Capacity > 0 &&
           _skillDetails.skills[0].skillActionDetails.skillActionType == SkillActionType.AOE)
            Gizmos.DrawWireSphere(transform.position, _skillDetails.skills[0].skillActionDetails.aoe.radius);
    }
    
    public void Initialize(SkillDetailsSO skillDetails)
    {
        ChildInitialize(skillDetails);
        AfterInitialize();
    }

    protected virtual void ChildInitialize(SkillDetailsSO skillDetails) { }

    private void AfterInitialize()
    {
        if (skillTimer == null)
        {
            Debug.LogError("Skill Timer is NULL");
            return;
        }
        _currentTime = 0;
        StartCoroutine(SkillTimeline());
    }

    protected virtual void ReturnToPoolAction() { }

    private IEnumerator SkillTimeline()
    {
        foreach (var action in _skillDetails.skills)
        {
            skillTimer.time = action.time - _currentTime;
            skillTimer.Play();
            yield return new WaitUntil(() => !skillTimer.isPlay);
            Debug.LogWarning("Next" + action.time);
            SkillAction(action.skillActionDetails);
            _currentTime += skillTimer.time;
        }
    }

    protected void SkillAction(SkillAction currentSkillAction)
    {
        switch (currentSkillAction.skillActionType)
        {
            case SkillActionType.AOE:
                
                var charactersList = TeamManager.Instance.CharactersList;
                
                // Find Character in Skill Range to Attack
                foreach (var character in charactersList)
                {
                    if(character == null) continue;
                    var distance = Vector3.Distance(character.transform.position, transform.position);
                    // Check in Skill Range
                    if(distance > currentSkillAction.aoe.radius) return;
                    character.gameObject.GetComponent<IAttack>().TakeDamage(currentSkillAction.damage);
                    foreach (var effect in currentSkillAction.onApplyEffects)
                        character.gameObject.GetComponent<CharacterStatus>().AddStatusEffect(_skillDetails, effect, 1);
                }
                break;
            case SkillActionType.Targeted:
                break;
            case SkillActionType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
