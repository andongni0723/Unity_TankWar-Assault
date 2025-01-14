using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : NetworkBehaviour, IAttack
{
    [Header("Component")]
    public Slider healthBar;
    public TMP_Text healthText;
    
    [Header("Settings")]
    public float maxHealth = 100;
    
    //[Header("Debug")]
    // private float currentHealth;
    private NetworkVariable<float> currentHealth = new(10, writePerm: NetworkVariableWritePermission.Owner);


    public override void OnNetworkDespawn()
    {
        EventHandler.CallOnPlayerDied(IsOwner);
    }
    
    private void Awake()
    {
        InitialData();
    }

    private void OnEnable()
    {
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void InitialData()
    {
        currentHealth.Value = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth.Value;
        healthText.text = currentHealth.Value + " / " + maxHealth; 
    }
    private void OnHealthChanged(float previousvalue, float newvalue)
    {
        healthBar.value = newvalue;
        healthText.text = newvalue + " / " + maxHealth;
        
        #if PLATFORM_ANDROID
        Handheld.Vibrate();
        #endif
        
        if (newvalue <= 0)
        {
            // gameObject.SetActive(false);
            // NetworkObject.Destroy(gameObject);
            RequestDespawnServerRpc();
        } 
    }
    
    [ServerRpc]
    private void RequestDespawnServerRpc()
    {
        NetworkObject.Despawn(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if(!IsOwner) return;
        
        currentHealth.Value -= damage;
        
    }
}
