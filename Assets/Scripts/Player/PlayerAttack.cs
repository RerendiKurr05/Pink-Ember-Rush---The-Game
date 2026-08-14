using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Referensi")]
    private PlayerPaintManager paintManager;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Statistik Serangan")]
    public float attackRange = 0.8f;
    public int attackDamage = 1;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    void Start()
    {
        paintManager = GetComponent<PlayerPaintManager>();
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
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        bool hasHitSomeone = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
            
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
                hasHitSomeone = true;
            }
        }

        if (hasHitSomeone)
        {
            GameJuiceManager.instance.HitStop(0.1f);
            
            GameJuiceManager.instance.ShakeCamera(4f, 0.15f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}