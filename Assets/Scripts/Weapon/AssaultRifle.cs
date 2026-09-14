using UnityEngine;
using TylerCode.SoundSystem;
public class AssaultRifle : Weapon
{
    public GameObject bulletPrefab;
    [SerializeField] private float spreadAngle = 5f; 
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
            float angle = Random.Range(-spreadAngle, spreadAngle);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle) * firePoint.rotation);
            bullet.GetComponent<Bullet>().SetFaction(faction);
            bullet.GetComponent<Bullet>().SetDamage(damage);
            bullet.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            fireCooldown = fireRate + 0.1f;
        }
    }

    void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }
}
