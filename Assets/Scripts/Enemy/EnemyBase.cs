using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Statistik")]
    public int maxHealth = 3;
    protected int currentHealth;
    public float moveSpeed = 3f;
    public bool isAttacking = false;

    [Header("Referensi")]
    protected Transform player;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("Efek Visual")]
    public GameObject heartPrefab;
    public GameObject heartBurstEffectPrefab; // <-- TAMBAHIN INI, efek partikel pas mati

    [Header("Knockback")]
    public float knockbackResistance = 1f;
    private bool isKnockedBack = false;
    private Vector2 knockbackVelocity;
    private float knockbackTimer;

    [Header("Damage")]
    public bool dealsContactDamage = true; // false = player gak kena damage cuma karena nyentuh

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (isKnockedBack)
        {
            HandleKnockback();
            return;
        }

        if (player != null)
        {
            Move();
        }
    }

    void HandleKnockback()
    {
        Vector3 positionBeforeMove = transform.position;

        transform.Translate(knockbackVelocity * Time.deltaTime, Space.World);

        if (transform.position.y < positionBeforeMove.y && knockbackVelocity.y <= 0)
        {
            transform.position = new Vector3(transform.position.x, positionBeforeMove.y, transform.position.z);
        }

        knockbackTimer -= Time.deltaTime;
        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, Time.deltaTime * 6f);

        if (knockbackTimer <= 0f)
        {
            isKnockedBack = false;
            knockbackVelocity = Vector2.zero;
        }
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        isKnockedBack = true;
        knockbackVelocity = direction.normalized * force * knockbackResistance;
        knockbackTimer = duration;
        OnKnockedBack();
    }

    protected virtual void OnKnockedBack() { } 
    protected virtual void Move() { }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        float healthRatio = (float)currentHealth / maxHealth;
        sr.color = Color.Lerp(Color.magenta, originalColor, healthRatio);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (heartBurstEffectPrefab != null)
        {
            Instantiate(heartBurstEffectPrefab, transform.position, Quaternion.identity);
        }

        if (heartPrefab != null)
        {
            Instantiate(heartPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}