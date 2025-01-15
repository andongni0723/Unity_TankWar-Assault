using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDetailPanelView : MonoBehaviour
{
    // [Header("Component")]
    public TMP_Text weaponNameText;
    public Image weaponImage;
    public TMP_Text shootingIntervalText;
    public TMP_Text bulletSpeedText;
    public TMP_Text bulletDamageText;
    public TMP_Text capacityText;
    public TMP_Text reloadTimeText;
    public TMP_Text featureText;
    public TMP_Text disadvantageText;
    //[Header("Settings")]
    //[Header("Debug")]
    
    /// <summary>
    /// This method Call by WeaponDetailPanelController.cs
    /// </summary>
    /// <param name="data"></param>
    public void UpdateWeaponDetailUI(WeaponDetailsSO data)
    {
        weaponNameText.text = data.weaponName;
        weaponImage.sprite ??= data.weaponSprite;
        shootingIntervalText.text = data.shootingInterval + "s";
        bulletSpeedText.text = data.projectileDetails.projectileSpeed.ToString();
        bulletDamageText.text = data.projectileDetails.projectileDamage.ToString();
        capacityText.text = data.capacity.ToString();
        reloadTimeText.text = data.reloadTime + "s";
        featureText.text = data.featureText;
        disadvantageText.text = data.disadvantageText;
    }
}
