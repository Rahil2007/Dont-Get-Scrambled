using UnityEngine;
using TylerCode.SoundSystem;
public class FlyingEnemy : Enemy
{
    private GameObject player;
    private Rigidbody2D rb;
    private Health healthScript;
    private MoneyManager moneyManager;
    private S4SoundSource soundSource;
    private SmoothShakeFree.SmoothShake smoothShake;
    [SerializeField] SmoothShakeFree.SmoothShakeFreePreset explodePreset;

    [SerializeField] ParticleSystem explodeParticle;

    private void Awake()
    {
        // Initialize variables
        smoothShake = FindAnyObjectByType<SmoothShakeFree.SmoothShake>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<Health>();
        if (healthScript != null)
            healthScript.OnDeath += Die;
        moneyManager = FindAnyObjectByType<MoneyManager>();
    }

    void Start()
    {
        soundSource = GetComponent<S4SoundSource>();

    }
    void Update()
    {
        if (player != null)
            FollowPlayer();
    }
    void FollowPlayer()
    {
        //Calculating dirn towards player and moving towards them with some randomness
        Vector2 dirn = player.transform.position - this.transform.position;
        dirn.Normalize();
        rb.linearVelocity = new Vector2(dirn.x * speed + Random.Range(-0.5f, 0.5f), dirn.y * speed + Random.Range(-0.5f, 0.5f));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        FactionMember hitInfo = collision.gameObject.GetComponent<FactionMember>();
        if (hitInfo != null && hitInfo.Faction == Faction.Player)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                smoothShake.StartShake(explodePreset);
                explodeParticle.transform.parent = null;
                explodeParticle.Play();
                soundSource.PlaySound("explosion");
                playerHealth.TakeDamage(contactDamage * GetComponent<Enemy>().damageMultiplier);
                Die();
            }
            //Apply A LOT of knockback
            Vector2 knockbackDirn = collision.gameObject.transform.position - this.transform.position;
            knockbackDirn.Normalize();
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockbackDirn * knockbackForce, ForceMode2D.Impulse);
        }
    }

    void Die()
    {
        smoothShake.StartShake(explodePreset);
        if (GetComponent<Enemy>().isDead) return;
        GetComponent<Enemy>().isDead = true;
        if (moneyManager != null)
            moneyManager.AddMoney(moneyDrop);
        WaveManager waveManager = FindAnyObjectByType<WaveManager>();
        if (waveManager != null) waveManager.NotifyDeath();
        Destroy(gameObject);
    }
    void OnDisable()
    {
        if (healthScript != null)
            healthScript.OnDeath -= Die;
    }
}
