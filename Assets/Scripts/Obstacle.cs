using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : PoolableObject, IAttack
{
    public bool isDestroyWhenHit = false;
    public void TakeDamage(int damage)
    {
        if (isDestroyWhenHit)
            ReturnToPool();
    }
}
