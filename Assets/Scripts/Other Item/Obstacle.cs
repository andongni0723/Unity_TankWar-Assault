using UnityEngine;

public class Obstacle : MonoBehaviour, IAttack
{
    public int health = 1; 
    public bool isDestructible = false;
    public void TakeDamage(int damage)
    {
        if(isDestructible) return;
        health -= damage;
        if (health <= 0)
            gameObject.SetActive(false);
    }
}
