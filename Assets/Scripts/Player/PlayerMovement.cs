using System;
using TylerCode.SoundSystem;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Faction faction = Faction.Player;
    private float horizontal;
    private bool isJumping;
    private Health healthScript;
    [SerializeField] UIHandler uIHandler;

    [Header("Movement")]
    [SerializeField] float maxGroundSpeed = 5f;
    [SerializeField] float maxAirSpeed = 5f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float deceleration = 5f;
    [SerializeField] float turnMult = 1.5f;

    [Header("Rotation")]
    private bool isRight = true;
    private Quaternion finalRotation;
    [SerializeField] float rotationSpeed = 720f;

    [Header("Jumping")]
    [SerializeField] float jumpPower = 5f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    [SerializeField] float baseGrav = 10f;  

    [Header("PlayerComponents")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    [Header("Extras")]
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBufferTime = 0.12f;
    [SerializeField] float stompDamage = 25f;
    [SerializeField] float stompMultiplier = 0.75f;
    float coyoteTimeCounter;
    float jumpBufferCounter;
    bool canDoubleJump = false;
    PlayerStats playerStats;
    private S4SoundSource soundManager;

    [Header("Particles/CameraShake")]
    [SerializeField] ParticleSystem jumpParticles;
    [SerializeField] SmoothShakeFree.SmoothShakeFreePreset stompPreset;
    [SerializeField] SmoothShakeFree.SmoothShakeFreePreset diePreset;
    private SmoothShakeFree.SmoothShake smoothShake;
    void Awake()
    {
        smoothShake = FindAnyObjectByType<SmoothShakeFree.SmoothShake>();
        healthScript = GetComponent<Health>();
        playerStats = GetComponent<PlayerStats>();
        if (healthScript != null)
            healthScript.OnDeath += Die;
    }

    void Start()
    {
        soundManager = GetComponent<S4SoundSource>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpBufferCounter = jumpBufferTime;
        }
        if (Input.GetKeyUp(KeyCode.Space))
            isJumping = false;

        if (isGrounded())
            coyoteTimeCounter = coyoteTime;

        else
            coyoteTimeCounter -= Time.deltaTime;

        if(jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

    }

    void FixedUpdate()
    {
        Movement();
        Jump();
        VariableGravity();        
        if (horizontal > 0f && !isRight)
            Flip();
        else if (horizontal < 0f && isRight)
            Flip();
        SmoothRotate();
    }
    void Movement()
    {
        float targetSpeed;
        if (!isGrounded() && rb.linearVelocityY < 0f)
            targetSpeed = horizontal * maxAirSpeed;
        else
            targetSpeed = horizontal * maxGroundSpeed;

        //float rate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float rate;
        if (targetSpeed > 0.01f && horizontal < -0.01f)
            rate = turnMult * deceleration;
        else if (targetSpeed < -0.01f && horizontal > 0.01f)
            rate = turnMult * deceleration;
        else if (Mathf.Abs(targetSpeed) > 0.01f)
            rate = acceleration;
        else
            rate = deceleration;
        float newVelocityX = Mathf.MoveTowards(rb.linearVelocityX, targetSpeed, rate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocityY);
    }

    void Jump()
    {
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            jumpParticles.Play();
            soundManager.PlaySound("jump");
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
            canDoubleJump = true;
        }
        else if(canDoubleJump && jumpBufferCounter > 0f && !isGrounded())
        {
            jumpParticles.Play();
            soundManager.PlaySound("jump");
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower * 0.85f);
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
            canDoubleJump = false;
        }
    }
    
    void VariableGravity()
    {
        if (rb.linearVelocityY < 0f)
            rb.gravityScale = baseGrav * fallMultiplier;
        else if (rb.linearVelocityY > 0f && !isJumping)
            rb.gravityScale = baseGrav * lowJumpMultiplier;
        else
            rb.gravityScale = baseGrav;
    }
    void Flip()
    {
        float targetAngle = isRight ? 180f : 0f;
        finalRotation = Quaternion.Euler(0f, targetAngle, 0f);
        isRight = !isRight;
    }

    void SmoothRotate()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void Die()
    {
        smoothShake.StartShake(diePreset);
        Invoke("GameOverUI", 0.25f); 
        gameObject.SetActive(false);
        Debug.Log("Player has died.");
    }

    void GameOverUI()
    {
        if (uIHandler != null)
            uIHandler.GameOver();
    }

    void OnDisable()
    {
        if (healthScript != null)
            healthScript.OnDeath -= Die;

    }
    void OnEnable()
    {
        if (healthScript != null)
            healthScript.OnDeath += Die;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
            return;
        FactionMember factionMember = collision.GetComponentInParent<FactionMember>();
        if(factionMember != null && factionMember.Faction == Faction.Enemy && rb.linearVelocityY <= 0f)
            Stomp(collision.GetComponentInParent<Health>());    
    }

    void Stomp(Health enemyHealth)
    {
        smoothShake.StartShake(stompPreset);
        jumpParticles.Play();
        canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
        GetComponent<Health>().SetInvincibility(true);
        Debug.Log("Stomped on enemy!");
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower * stompMultiplier);
        canDoubleJump = true;
        enemyHealth.TakeDamage(stompDamage * playerStats.DamageMultiplier);
    }
}