using UnityEngine;
using TylerCode.SoundSystem;

public class Shotgun : Weapon
{
    public GameObject pelletPrefab;
    private float fireCooldown = 0f;
    [SerializeField] int noOfPellets = 5;
    [SerializeField] float recoil;
    [SerializeField] float spreadAngle = 15f;
    private S4SoundSource soundSource;

    private GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        soundSource = GetComponent<S4SoundSource>();
    }

    void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    public override void Fire(Faction faction)
    {
        if (fireCooldown <= 0f) { 
            soundSource.PlaySound("Shoot");
            //Creating a burst of pellets
            for (int i = 0; i < noOfPellets; i++)
            {
                //For uniform spread, we can use following formula.
                float offset = 3.5f;
                float angle = (-spreadAngle + (2 * spreadAngle / (noOfPellets - 1)) * i) + Random.Range(-offset, offset);
                GameObject bullet = Instantiate(pelletPrefab, transform.position, Quaternion.Euler(0, 0, angle) * transform.rotation);
                bullet.GetComponent<Bullet>().SetFaction(faction);
                bullet.GetComponent<Bullet>().SetDamage(damage);
                bullet.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            }
            fireCooldown = fireRate + 0.3f;
            if (player != null)
                player.GetComponent<Rigidbody2D>().AddForce(-transform.right * recoil, ForceMode2D.Impulse);
        }
    }
}