using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitVFX : PoolableObject
{
    //[Header("Component")]
    [Header("Settings")]
    public Timer destroyTimer;
    //[Header("Debug")]
    
    private void Awake() => destroyTimer.OnTimerEnd += ReturnToPool;
}
