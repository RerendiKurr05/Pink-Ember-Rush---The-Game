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

    private int facingDir = 1;
    private float verticalVelocity = 0f;
    public bool isGrounded;

    protected override void Move()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        int desiredDir = direction.x >= 0 ? 1 : -1;

        if (desiredDir != facingDir)
        {
            facingDir = desiredDir;
            UpdateFacing();
        }

        float horizontalMove = IsBlocked() ? 0f : facingDir * moveSpeed;

        // Horizontal jalan seperti biasa
        transform.Translate(new Vector2(horizontalMove, 0f) * Time.deltaTime);

        // Vertical/gravity ditangani terpisah, pakai sweep-check biar gak tunneling
        ApplyGravityWithSweepCheck();
    }

    void ApplyGravityWithSweepCheck()
    {
        // Jarak raycast = seberapa jauh dia BAKAL turun frame ini, plus buffer kecil
        float projectedFallDistance = Mathf.Abs(verticalVelocity) * Time.deltaTime;
        float castDistance = Mathf.Max(groundCheckDistance, projectedFallDistance + 0.05f);

        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, castDistance, groundLayer);

        if (hit.collider != null && verticalVelocity <= 0f)
        {
            isGrounded = true;
            verticalVelocity = 0f;

            // Snap posisi PERSIS di atas permukaan, gak cuma direm pelan-pelan
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
    }
}