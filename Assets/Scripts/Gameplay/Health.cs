using System;
using UnityEngine;
using TylerCode.SoundSystem;
using Unity.VisualScripting;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    public float MaxHealth => maxHealth;
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    [SerializeField] int regenTimer;
    private float currentRegenTimer;
    [SerializeField] float regenRate;
    [SerializeField] bool canRegen = false;

    [SerializeField] private Animator animator;

    [SerializeField] private float maxInvincibilityDuration = 0f;
    [SerializeField] ParticleSystem deathParticleSystem;
    private float currentInvincibilityDuration = 0f;
    private bool isInvincible = false, startRegen = false;
    public event Action OnDeath, OnHealthChanged, OnMaxHealthChanged;
    private S4SoundSource soundSource;
    private SmoothShakeFree.SmoothShake smoothShake;
    [SerializeField] SmoothShakeFree.SmoothShakeFreePreset hitPreset;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        soundSource = GetComponent<S4SoundSource>();
        smoothShake = FindAnyObjectByType<SmoothShakeFree.SmoothShake>();
    }

    void Update()
    {
        if (canRegen && currentHealth < maxHealth && currentRegenTimer <= 0f && startRegen)
        {
            currentHealth = Mathf.Min(maxHealth, Mathf.MoveTowards(currentHealth, maxHealth, regenRate * Time.deltaTime));
            OnHealthChanged?.Invoke();
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
                currentRegenTimer = regenTimer;
            }
        }
        else if (currentRegenTimer > 0f)
        {
            currentRegenTimer -= Time.deltaTime;
        }
        if (isInvincible)
        {
            if (currentInvincibilityDuration <= 0f)
            {
                isInvincible = false;
                startRegen = true;
            }
            currentInvincibilityDuration -= Time.deltaTime;
        }
    }
    public void TakeDamage(float damage)
    {
        if (isInvincible && maxInvincibilityDuration > 0f)
            return;
        currentInvincibilityDuration = maxInvincibilityDuration;
        isInvincible = true;
        startRegen = false;
        currentRegenTimer = regenTimer;
        if (currentHealth > 0f)
        {
            if(hitPreset != null && smoothShake != null)
                smoothShake.StartShake(hitPreset);
            soundSource.PlaySound("hurt");
            if (animator != null)
                animator.SetTrigger("Hit");
            currentHealth -= damage;
            OnHealthChanged?.Invoke();
        }
        if (currentHealth <= 0f)
        {
            if (deathParticleSystem != null)
            {
                deathParticleSystem.transform.parent = null;
                deathParticleSystem.Play();
            }
            soundSource.PlaySound("death");
            OnDeath?.Invoke();
        }
    }

    public void SetInvincibility(bool value)
    {
        isInvincible = value;
    }

    public void UpdateMaxHealth(float newMaxHealth)
    {
       maxHealth = (int)newMaxHealth;
       currentHealth = maxHealth;
       OnMaxHealthChanged?.Invoke();
    }

    public void UpdateInvincibility(float newInvincibilityDuration)
    {
        maxInvincibilityDuration = newInvincibilityDuration;
    }
}