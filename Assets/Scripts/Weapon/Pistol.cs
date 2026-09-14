using UnityEngine;
using TylerCode.SoundSystem;
public class Pistol : Weapon
{
    public GameObject bulletPrefab;
    private float fireCooldown = 0f;
    private S4SoundSource soundSource;

    void Start()
    {
        soundSource = GetComponent<S4SoundSource>();
    }   
    public override void Fire(Faction faction)
    {
        if (fireCooldown <= 0f)
        {
            soundSource.PlaySound("Shoot");
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Bullet>().SetFaction(faction);
            bullet.GetComponent<Bullet>().SetDamage(damage);
            bullet.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            fireCooldown = fireRate + 0.2f;
        }
    }

    void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }
}
