using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetailsSO", menuName = "ScriptableObject/WeaponDetailsSO")]
public class WeaponDetailsSO : ScriptableObject
{
    public string weaponName;
    public string weaponID;
    public Sprite weaponSprite;
    public float shootingInterval;
    public float capacity;
    public float reloadTime;
    public ProjectileDetailsSO projectileDetails;
    [TextArea] public string featureText;
    [TextArea] public string disadvantageText; 
}
