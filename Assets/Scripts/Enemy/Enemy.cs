using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float speed = 2f;
    public float contactDamage = 10f;
    public float knockbackForce = 5f;
    public int moneyDrop = 10;
    public float damageMultiplier = 1f;
    // Track death to avoid double-calling Die / NotifyDeath
    [HideInInspector] public bool isDead = false;
}
