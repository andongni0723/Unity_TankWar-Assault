using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponDetailsSO", menuName = "ScriptableObject/WeaponDetailsSO")]
public class WeaponDetailsSO : ScriptableObject
{
    [PreviewField(100, ObjectFieldAlignment.Left), HideLabel]
    public Sprite weaponSprite;
    public string weaponName;
    public string weaponID;
    public float shootingInterval;
    public float spreadAngle;
    public bool infiniteAmmo;
    public int capacity;
    public float reloadTime;
    public ProjectileDetailsSO projectileDetails;
    [TextArea] public string featureText;
    [TextArea] public string disadvantageText; 
}
