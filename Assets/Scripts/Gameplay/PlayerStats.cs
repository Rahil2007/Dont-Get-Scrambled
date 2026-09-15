using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]private float maxHealth, fireRateMultiplier, damageMultiplier, maxInvincibility;
    public float MaxHealth => maxHealth;
    public float FireRateMultiplier => fireRateMultiplier;
    public float DamageMultiplier => damageMultiplier;
    public float MaxInvincibility => maxInvincibility;
    private WeaponHandler weaponHandler;
    private Health health;
    void Awake()
    {
        health = GetComponent<Health>();
        weaponHandler = GetComponent<WeaponHandler>();
        health.UpdateMaxHealth(maxHealth);
        health.UpdateInvincibility(maxInvincibility);
    }


    //Different methods to set different player stats, and updating them in their respective scripts
    public void SetMaxHealth(float value)
    {
        maxHealth = health.MaxHealth + value;
        Debug.Log($"MaxHealth: {MaxHealth}");
        health.UpdateMaxHealth(maxHealth);
    }

    public void SetFireRateMultiplier(float value)
    {
        fireRateMultiplier += value;
        Debug.Log($"FireRateMultiplier: {FireRateMultiplier}");
        weaponHandler.UpdateFireRate();

    }

    public void SetDamageMultiplier(float value)
    {
        damageMultiplier += value;
        Debug.Log($"DamageMultiplier: {DamageMultiplier}");
        weaponHandler.UpdateDamage();
    }

    public void SetMaxInvincibility(float value)
    {
        maxInvincibility += value;
        Debug.Log($"MaxInvincibility: {MaxInvincibility}");
        health.UpdateInvincibility(maxInvincibility);
    }
}