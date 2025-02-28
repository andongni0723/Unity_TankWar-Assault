using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour, IAttack
{
    public bool isActive = true;
    public GameObject model;
    public int maxHealth = 1;
    public int currentHealth;
    public bool isDestructible = false;
    public Timer respawnTimer;
    
    private Collider _collider;

    [Header("UI")] 
    public bool openUI;
    [ShowIf("openUI")] public Slider healthBar;
    [ShowIf("openUI")] public TMP_Text healthText;

    protected virtual void Awake()
    {
        if (!isDestructible && model == null)
        {
            model = gameObject;
            Debug.LogWarning("Obstacle: " + name + " has no model assigned");
        }
        else if (model == null) 
            model = gameObject;

        InitialComponent();
        ModelOnEnable();
    }
    
    protected virtual void InitialComponent()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        if(respawnTimer != null)
            respawnTimer.OnTimerEnd += ModelOnEnable;
    }
    
    private void OnDisable()
    {
        if(respawnTimer != null)
            respawnTimer.OnTimerEnd -= ModelOnEnable;
    }

    protected virtual void ModelOnEnable()
    {
        _collider.enabled = true;
        currentHealth = maxHealth;
        UpdateUI();
        model.SetActive(true);
    }
    
    protected async void ModelOnDisable()
    {
        _collider.enabled = false;
        await ModelBeforeDisable();
        model.SetActive(false);
    }

    protected virtual async Task ModelBeforeDisable()
    {
        await Task.CompletedTask;
    }

    private void UpdateUI()
    {
        if (!openUI) return;
        healthBar.value = currentHealth;
        healthText.text = currentHealth + " / " + maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        if(isDestructible) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            ModelOnDisable();
            if(respawnTimer != null)
                respawnTimer.Play();
        }
    }
}
