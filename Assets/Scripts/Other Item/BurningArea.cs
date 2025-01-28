using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BurningArea : PoolableObject
{
    //[Header("Component")]
    [Header("Settings")]
    private SkillDetailsSO _skillDetails;
    public Timer destroyTimer;
    //[Header("Debug")]

    public void Initialize(SkillDetailsSO skillDetails)
    {
        _skillDetails = skillDetails;
        transform.localScale = _skillDetails.skillRange;
        destroyTimer.time = _skillDetails.skillLifeTime;
    }

    private void Awake()
    {
        destroyTimer.OnTimerEnd += ReturnToPoolAction;
    }

    private void ReturnToPoolAction()
    {
        transform.DOScale(Vector3.zero, 0.3f).OnComplete(ReturnToPool);
    }
}
