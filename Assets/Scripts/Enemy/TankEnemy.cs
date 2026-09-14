using UnityEngine;

public class TankEnemy : Enemy
{
    private GameObject player;
    private Rigidbody2D rb;
    private Health healthScript;
    private MoneyManager moneyManager;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<Health>();
        if (healthScript != null)
            healthScript.OnDeath += Die;
        moneyManager = FindAnyObjectByType<MoneyManager>();
    }
    void Update()
    {
        if (player != null)
            FollowPlayer();
    }
    void FollowPlayer()
    {
        Vector2 dirn = player.transform.position - this.transform.position;
        dirn.Normalize();
        rb.linearVelocity = new Vector2(dirn.x * speed + Random.Range(-0.5f, 0.5f), rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        FactionMember hitInfo = collision.gameObject.GetComponent<FactionMember>();
        if (hitInfo != null && hitInfo.Faction == Faction.Player)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
            }
            Vector2 knockbackDirn = collision.gameObject.transform.position - this.transform.position;
            knockbackDirn.Normalize();
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockbackDirn * knockbackForce, ForceMode2D.Impulse);
        }
    }

    void Die()
    {
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
