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
    private DamageSpawner _damageSpawner;
    public Slider healthBar;
    public TMP_Text healthText;
    public Timer respawnTimer;
    
    [Header("Settings")]
    public float maxHealth = 100;
    
    //[Header("Debug")]
    // private float currentHealth;
    private NetworkVariable<float> currentHealth = new(10, writePerm: NetworkVariableWritePermission.Owner);


    private void Awake()
    {
        InitialData();
        int a = 10;
        int b = 20;

        (a, b) = (b, a);
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

    private void Update()
    {
        if (!IsOwner) return;

        if (respawnTimer.isPlay)
            GameUIManager.Instance.RespawnTimerText.text = ((int)respawnTimer.time - (int)respawnTimer.currentTime).ToString("0");
    }


    private void InitialData()
    {
        currentHealth.Value = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth.Value;
        healthText.text = currentHealth.Value + " / " + maxHealth; 
        _cc = GetComponent<CharacterController>();
        _damageSpawner = GetComponent<DamageSpawner>();
        respawnTimer.OnTimerEnd += Respawn;
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
            // RequestDespawnServerRpc();
            PlayerDied();
            respawnTimer.Play();
        } 
    }

    private void PlayerDied()
    {
        EventHandler.CallOnPlayerDied(IsOwner);
        _cc.virtualCamera.gameObject.SetActive(false);
        transform.position = Vector3.down * 100;
    }
    
    private void Respawn()
    {
        EventHandler.CallOnPlayerRespawn(IsOwner);
        _cc.InitialSpawnPoint();

        if (IsOwner)
        {
            _cc.virtualCamera.gameObject.SetActive(true);
            currentHealth.Value = maxHealth;
        }
    }
    
    private void OnGameEnd(bool isHost)
    {
        if (!IsOwner) return;
        
        if(AreaManager.Instance.GetWinnerTeam() == _cc.team.Value)
            GameUIManager.Instance.winPanel.SetActive(true);
        else
            GameUIManager.Instance.losePanel.SetActive(true);   
    }

    [ServerRpc]
    private void RequestDespawnServerRpc()
    {
        NetworkObject.Despawn(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (!IsOwner) return;

        currentHealth.Value -= damage;
        _damageSpawner.SpawnDamageText(-damage, transform.position);
}
}
