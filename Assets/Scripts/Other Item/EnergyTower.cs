using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.VFX;

[RequireComponent(typeof(NetworkObject))]
public class EnergyTower : NetworkBehaviour, IAttack
{
    [Header("Component")] 
    private BoxCollider _boxCollider;
    public GameObject tower;
    public VisualEffect redLaserVFX;
    public VisualEffect blueLaserVFX;
    public VisualEffect destroyVFX;
    
    [Header("UI")]
    public Slider healthBar;
    public TMP_Text healthText;
    
    [Header("Data")]
    public int maxHealth = 100;
    public NetworkVariable<Team> team = new(Team.None, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<AreaName> areaName = new(AreaName.None, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentHealth = new(0, writePerm: NetworkVariableWritePermission.Server);

    //[Header("Settings")]
    //[Header("Debug")]

    public override void OnNetworkSpawn() => StartCoroutine(SpawnInitialize());

    private IEnumerator SpawnInitialize()
    {
        _boxCollider = GetComponent<BoxCollider>();
        yield return new WaitUntil(() => team.Value != Team.None);
        tag = team.Value == Team.Blue ? "Blue Skill" : "Red Skill";
        gameObject.layer = team.Value == Team.Blue ? LayerMask.NameToLayer("Blue Skill") : LayerMask.NameToLayer("Red Skill");
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;
        PlayVFX(team.Value == Team.Red ? redLaserVFX : blueLaserVFX);
        currentHealth.OnValueChanged += (oldValue, newValue) => UpdateUI(newValue);
        UpdateUI(currentHealth.Value);
    }


    /// <summary>
    /// Call by AreaController.cs
    /// </summary>
    /// <param name="targetTeam">Tower owner's team</param>
    public void Initial(Team targetTeam, AreaName targetAreaName)
    {
        currentHealth.Value = maxHealth;
        team.Value = targetTeam;
        areaName.Value= targetAreaName;
    }

    private void PlayVFX(VisualEffect vfx)
    {
        vfx.gameObject.SetActive(true);
        vfx.Play();
    }
    
    
    private void UpdateUI(int newValue)
    {
        healthBar.value = newValue;
        healthText.text = newValue + " / " + maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if(!IsServer) return;
        currentHealth.Value -= damage;
        
        if (currentHealth.Value <= 0 && currentHealth.Value != -1000)
        {
            StartCoroutine(TowerDestroy());
            currentHealth.Value = -1000;
        }
    }

    private IEnumerator TowerDestroy()
    {
        if (!IsServer) yield break;
        DestroyAnimationClientRpc();
        DestroyAnimation();
        yield return new WaitForSeconds(1.5f); // Wait animation end
        GetComponent<NetworkObject>().Despawn();
    }
    
    [ClientRpc] 
    private void DestroyAnimationClientRpc()
    {
        if(IsServer) return;
        DestroyAnimation();
    }

    private void DestroyAnimation()
    {
        _boxCollider.enabled = false;
        PlayVFX(destroyVFX);
        tower.SetActive(false); 
        EventHandler.CallOnEnergyTowerDestroyed(areaName.Value);
    }
}
