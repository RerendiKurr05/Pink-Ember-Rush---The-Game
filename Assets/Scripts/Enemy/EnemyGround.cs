using UnityEngine;

public class EnemyGround : EnemyBase
{
    [Header("Deteksi Wall & Edge")]
    public LayerMask groundLayer;
    public float wallCheckDistance = 0.3f;
    public float edgeCheckDistance = 0.5f;
    public Transform wallCheckPoint;
    public Transform edgeCheckPoint;

    [Header("Gravitasi")]
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.1f;
    public float gravity = 20f;
    public float maxFallSpeed = 15f;

    [Header("Serangan Seruduk (centang kalau punya animasi attack)")]
    public bool usesLungeAttack = false;
    public Animator animator;               // Animator enemy ini
    public LayerMask playerLayer;           // layer Player
    public Transform attackHitPoint;        // child di depan kepala enemy (boleh kosong)
    public float attackHitRadius = 0.5f;
    public int attackDamage = 1;
    public float attackTriggerRange = 1.5f; // jarak ke player yang memicu seruduk
    public float windupTime = 0.3f;         // frame awal: ancang-ancang
    public float lungeTime = 0.2f;          // frame tengah: badan maju
    public float lungeSpeed = 8f;
    public float recoverTime = 0.3f;        // frame akhir: balik ke posisi semula
    public float attackCooldown = 1.5f;

    private enum State { Chase, Windup, Lunge, Recover }
    private State state = State.Chase;
    private float stateTimer;
    private float nextAttackTime;
    private bool hitDealt;

    private int facingDir = 1;
    private float verticalVelocity = 0f;
    public bool isGrounded;

    protected override void Start()
    {
        base.Start();

        if (animator == null) animator = GetComponent<Animator>();

        // Enemy seruduk: damage dari seruduknya, bukan dari sentuhan biasa
        if (usesLungeAttack) dealsContactDamage = false;
    }

    protected override void Move()
    {
        if (player == null) return;

        switch (state)
        {
            case State.Chase:
                HandleChase();
                break;

            case State.Windup:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    state = State.Lunge;
                    stateTimer = lungeTime;
                    hitDealt = false;
                }
                break;

            case State.Lunge:
                HandleLunge();
                break;

            case State.Recover:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    state = State.Chase;
                    nextAttackTime = Time.time + attackCooldown;
                }
                break;
        }

        // Gravitasi selalu jalan di semua state
        ApplyGravityWithSweepCheck();
    }

    void HandleChase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        int desiredDir = direction.x >= 0 ? 1 : -1;

        if (desiredDir != facingDir)
        {
            facingDir = desiredDir;
            UpdateFacing();
        }

        float horizontalMove = IsBlocked() ? 0f : facingDir * moveSpeed;
        transform.Translate(new Vector2(horizontalMove, 0f) * Time.deltaTime);

        if (usesLungeAttack && Time.time >= nextAttackTime && isGrounded)
        {
            float dx = Mathf.Abs(player.position.x - transform.position.x);
            float dy = Mathf.Abs(player.position.y - transform.position.y);

            if (dx <= attackTriggerRange && dy <= 1.5f)
            {
                StartWindup();
            }
        }
    }

    void StartWindup()
    {
        state = State.Windup;
        stateTimer = windupTime;
        if (animator != null) animator.SetTrigger("Attack");
    }

    void HandleLunge()
    {
        // Maju ke depan, tapi berhenti kalau mentok tembok / ujung platform
        if (!IsBlocked())
        {
            transform.Translate(new Vector2(facingDir * lungeSpeed, 0f) * Time.deltaTime);
        }

        TryHitPlayer();

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            state = State.Recover;
            stateTimer = recoverTime;
        }
    }

    void TryHitPlayer()
    {
        if (hitDealt) return; // 1x seruduk = maksimal 1x damage

        Vector2 center = attackHitPoint != null
            ? (Vector2)attackHitPoint.position
            : (Vector2)transform.position + Vector2.right * facingDir * 0.5f;

        Collider2D hit = Physics2D.OverlapCircle(center, attackHitRadius, playerLayer);
        if (hit == null) return;

        PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            hitDealt = true;
        }
    }

    // Kena tembakan cat saat lagi nyeruduk -> serangan dibatalkan
    protected override void OnKnockedBack()
    {
        state = State.Chase;
        nextAttackTime = Time.time + attackCooldown;
    }

    void ApplyGravityWithSweepCheck()
    {
        float projectedFallDistance = Mathf.Abs(verticalVelocity) * Time.deltaTime;
        float castDistance = Mathf.Max(groundCheckDistance, projectedFallDistance + 0.05f);

        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, castDistance, groundLayer);

        if (hit.collider != null && verticalVelocity <= 0f)
        {
            isGrounded = true;
            verticalVelocity = 0f;

            float feetOffset = transform.position.y - groundCheckPoint.position.y;
            transform.position = new Vector3(transform.position.x, hit.point.y + feetOffset, transform.position.z);
        }
        else
        {
            isGrounded = false;
            verticalVelocity -= gravity * Time.deltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, -maxFallSpeed);
            transform.Translate(new Vector2(0f, verticalVelocity * Time.deltaTime));
        }
    }

    bool IsBlocked()
    {
        bool wallAhead = wallCheckPoint != null &&
            Physics2D.Raycast(wallCheckPoint.position, Vector2.right * facingDir, wallCheckDistance, groundLayer);

        bool groundAhead = edgeCheckPoint != null &&
            Physics2D.Raycast(edgeCheckPoint.position, Vector2.down, edgeCheckDistance, groundLayer);

        return wallAhead || !groundAhead;
    }

    void UpdateFacing()
    {
        transform.localScale = new Vector3(facingDir, 1, 1);
    }

    void OnDrawGizmosSelected()
    {
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.right * facingDir * wallCheckDistance);
        }
        if (edgeCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(edgeCheckPoint.position, edgeCheckPoint.position + Vector3.down * edgeCheckDistance);
        }
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }
        if (attackHitPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackHitPoint.position, attackHitRadius);
        }
    }
}