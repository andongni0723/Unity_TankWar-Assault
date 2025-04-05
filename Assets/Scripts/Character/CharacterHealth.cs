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
    private CharacterController _cc;
    private CharacterShoot _shoot;
    private DamageSpawner _damageSpawner;
    public Slider healthBar;
    public TMP_Text healthText;
    public Timer respawnTimer;
    
    [Header("Settings")]
    public float maxHealth = 100;
    
    //[Header("Debug")]
    // private float currentHealth;
    private NetworkVariable<float> currentHealth = new(10, writePerm: NetworkVariableWritePermission.Server);


    private void Awake()
    {
        InitializeComponent();
    }

    private void OnEnable()
    {
        currentHealth.OnValueChanged += OnHealthChanged;
        EventHandler.OnGameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
        EventHandler.OnGameEnd -= OnGameEnd;
    }

    private void InitializeComponent()
    {
        currentHealth.Value = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth.Value;
        healthText.text = currentHealth.Value + " / " + maxHealth; 
        _cc = GetComponent<CharacterController>();
        _shoot = GetComponent<CharacterShoot>();
        _damageSpawner = GetComponent<DamageSpawner>();
        respawnTimer.OnTimerEnd += Respawn;
    }

    private void OnHealthChanged(float previousValue, float newValue)
    {
        healthBar.value = newValue;
        healthText.text = newValue + " / " + maxHealth;
        
        // #if PLATFORM_ANDROID && !UNITY_EDITOR
        // // VibratorHelper.Vibrate(500, 0.3f);
        // // Handheld.Vibrate();
        // #else
        // Handheld.Vibrate();
        // #endif
        
        EventHandler.CallCameraShake(5, 0.5f, 0);
        Handheld.Vibrate();
        
        if (newValue <= 0)
        {
            PlayerDied();
            respawnTimer.Play();
        } 
    }

    private void PlayerDied()
    {
        EventHandler.CallOnPlayerDied(IsOwner, respawnTimer);
        _cc.virtualCamera.gameObject.SetActive(false);
        transform.position = Vector3.down * 100;
    }
    
    private void Respawn()
    {
        EventHandler.CallOnPlayerRespawn(IsOwner);
        _cc.InitialSpawnPoint();
        _shoot.CallAllWeaponReload();

        if (!IsOwner) return;
        _cc.virtualCamera.gameObject.SetActive(true);
        RespawnHealthServerRpc();
    }
    
    private void OnGameEnd(bool isHost)
    {
        if (!IsOwner) return;
        
        if(AreaManager.Instance.GetWinnerTeam() == _cc.team.Value)
            GameUIManager.Instance.winPanel.SetActive(true);
        else
            GameUIManager.Instance.losePanel.SetActive(true);   
    }
    
    public void TakeDamage(int damage)
    {
        if (_cc.IsOwner) return;
        Debug.Log("Hurt");
        DamageServerRpc(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DamageServerRpc(int damage)
    {
        Debug.Log("Hurt DamageServerRpc");
        currentHealth.Value -= damage;
        _damageSpawner.CallSpawnDamageText(-damage, transform.position);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RespawnHealthServerRpc()
    {
        currentHealth.Value = maxHealth;
    }
}
