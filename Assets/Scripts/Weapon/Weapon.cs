using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage;
    public float fireRate;
    public bool isSingleUse;
    public bool holdFire;
    public Transform firePoint;
    public float bulletSpeed;

    public abstract void Fire(Faction faction);
}