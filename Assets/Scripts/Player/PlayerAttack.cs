using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Referensi")]
    private PlayerPaintManager paintManager;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject projectilePrefab;

    [Header("Statistik Serangan")]
    public float attackRange = 0.8f;
    public int attackDamage = 1;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    [Header("Arah Hadap Saat Menyerang")]
    public float aimFaceDuration = 0.4f; // berapa lama player tetap hadap ke arah tembakan
    private float aimFaceTimer = 0f;
    private SpriteRenderer playerSprite;

    public bool IsAimLocked => aimFaceTimer > 0f;

    private Camera mainCamera;

    void Start()
    {
        paintManager = GetComponent<PlayerPaintManager>();
        mainCamera = Camera.main;
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (aimFaceTimer > 0f) aimFaceTimer -= Time.deltaTime;
        if (paintManager.isAttackModeActive)
        {
            if (Time.time >= nextAttackTime)
            {
                if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J))
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
        }
    }

    void Attack()
    {
        paintManager.playerAnimator.SetTrigger("Attack");

        if (projectilePrefab == null)
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            attackPoint.position,
            Quaternion.identity
        );

        Vector2 direction = GetMouseDirection();
        // Hadapkan player ke arah mouse
        if (playerSprite != null && Mathf.Abs(direction.x) > 0.01f)
        {
            playerSprite.flipX = direction.x < 0;
            aimFaceTimer = aimFaceDuration;
        }

        PaintProjectile projectileScript = projectile.GetComponent<PaintProjectile>();

        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
        }

        // Flip visual proyektil kalau arah dominan ke kiri
        if (direction.x < 0)
        {
            projectile.transform.localScale = new Vector3(
                -Mathf.Abs(projectile.transform.localScale.x),
                projectile.transform.localScale.y,
                projectile.transform.localScale.z
            );
        }
    }

    Vector2 GetMouseDirection()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z - attackPoint.position.z);

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = attackPoint.position.z;

        Vector2 direction = (mouseWorldPos - attackPoint.position).normalized;
        return direction;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}