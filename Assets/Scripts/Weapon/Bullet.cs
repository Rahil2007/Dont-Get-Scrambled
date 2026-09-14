using TylerCode.SoundSystem;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 20f; // default speed value, can be set via SetSpeed method
    [SerializeField] float lifetime = 2f;
    [SerializeField] ParticleSystem hitEffect;
    private S4SoundSource soundSource;
    private Rigidbody2D rb;
    private float lifeTimer;
    private Faction myFaction;
    private FactionMember bulletFaction;

    private float damage = 10f; // Default damage value, can be set via SetDamage method

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeTimer = lifetime;
        soundSource = GetComponent<S4SoundSource>();
        bulletFaction = GetComponent<FactionMember>();
    }


    void Update()
    {
        rb.linearVelocity = transform.right * speed;
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetFaction(Faction faction)
    {
        myFaction = faction; 
        bulletFaction.SetMyFaction(faction);
    }

    public void SetDamage (float damage)
    {
        // Implement damage logic if needed
        this.damage = damage;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        FactionMember hitInfo = collision.GetComponent<FactionMember>();
        if (hitInfo != null && hitInfo.Faction != myFaction && !collision.CompareTag("Bullet"))
        {
            // Add Hit Effect + Enemy Damage Logic Here
            Quaternion q = hitEffect.transform.rotation;
            q = Quaternion.Euler(Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg, -90, 90);
            hitEffect.transform.rotation = q;
            hitEffect.transform.parent = null;
            hitEffect.Play();
            soundSource.PlaySound("hit");
            Destroy(gameObject);
            hitInfo.GetComponent<Health>().TakeDamage(damage);
        }
        else if ((collision.CompareTag("Obstacle") && hitInfo == null) || (collision.CompareTag("Bullet") && hitInfo.Faction != myFaction))
        {
            // Add Hit Effect Here
            Quaternion q = hitEffect.transform.rotation;
            q = Quaternion.Euler(Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x)* Mathf.Rad2Deg, -90, 90);
            hitEffect.transform.rotation = q;
            hitEffect.transform.parent = null;
            hitEffect.Play();
            soundSource.PlaySound("hit");
            Destroy(gameObject);
        }
    }
}