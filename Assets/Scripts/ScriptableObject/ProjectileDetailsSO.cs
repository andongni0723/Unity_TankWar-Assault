using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDetailsSO", menuName = "ScriptableObject/ProjectileDetailsSO")]
public class ProjectileDetailsSO : ScriptableObject
{
    public string projectileName;
    public float shootingInterval;
    public float projectileSpeed;
    public float projectileLifeTime;
    public float projectileDamage;
    public float capacity;
    public float reloadTime;
    public string featureText;
    public string disadvantageText;
}
