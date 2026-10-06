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

    private Camera mainCamera;

    void Start()
    {
        paintManager = GetComponent<PlayerPaintManager>();
        mainCamera = Camera.main;
    }

    void Update()
    {
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