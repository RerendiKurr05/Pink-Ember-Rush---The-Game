using UnityEngine;

public class EnemyFlying : EnemyBase
{
    [Header("Deteksi Wall (Horizontal)")]
    public LayerMask obstacleLayer;
    public float wallCheckDistance = 1f;

    [Header("Serangan (Jatuhkan Objek)")]
    public GameObject dropPrefab;
    public float dropRange = 2f;
    public float dropCooldown = 2f;
    private float nextDropTime;

    private int facingDir = 1;
    private float fixedY;

    protected override void Start()
    {
        base.Start();
        fixedY = transform.position.y; // Y dikunci sesuai posisi spawn awal, gak berubah lagi
    }

    protected override void Move()
    {
        if (player == null) return;

        HandleHorizontalMovement();
        HandleAttack();

        // Pastikan Y selalu balik ke nilai terkunci (jaga-jaga dari drift)
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }

    void HandleHorizontalMovement()
    {
        float directionX = player.position.x - transform.position.x;
        int desiredDir = directionX >= 0 ? 1 : -1;

        if (desiredDir != facingDir)
        {
            facingDir = desiredDir;
            transform.localScale = new Vector3(facingDir, 1, 1);
        }

        bool wallAhead = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, obstacleLayer);
        bool alreadyCloseEnough = Mathf.Abs(directionX) < 0.1f;

        if (!wallAhead && !alreadyCloseEnough)
        {
            transform.Translate(Vector2.right * facingDir * moveSpeed * Time.deltaTime);
        }
    }

    void HandleAttack()
    {
        if (dropPrefab == null) return;
        if (Time.time < nextDropTime) return;

        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);

        if (horizontalDistance <= dropRange)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
            nextDropTime = Time.time + dropCooldown;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * facingDir * wallCheckDistance);
    }
}