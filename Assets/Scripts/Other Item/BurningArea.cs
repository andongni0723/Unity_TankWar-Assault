using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BurningArea : SkillBase
{
    [Header("Component")] 
    public GameObject fireParticle;
    public GameObject smokeParticle;
    
    // [Header("Settings")]
    // [Header("Debug")]

    protected override void ChildInitialize(SkillDetailsSO skillDetails)
    {
        _skillDetails = skillDetails;
        transform.localScale = skillDetails.skillRange;
        fireParticle.transform.localScale = Vector3.one;
        smokeParticle.transform.localScale = Vector3.one;
        destroyTimer.time = _skillDetails.skillLifeTime;
    }

    protected override void ReturnToPoolAction()
    {
        fireParticle.transform.DOScale(Vector3.zero, 0.1f);
        smokeParticle.transform.DOScale(Vector3.zero, 0.1f);
        transform.DOScale(Vector3.zero, 0.3f).OnComplete(ReturnToPool);
    }


    private void OnTriggerEnter(Collider other)
    {
        
    }
}
