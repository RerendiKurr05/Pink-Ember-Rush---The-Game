using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Statistik")]
    public int maxHealth = 3;
    protected int currentHealth;
    public float moveSpeed = 3f;

    [Header("Referensi")]
    protected Transform player;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("Efek Visual")]
    public GameObject heartPrefab;

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
        if (player != null)
        {
            Move();
        }
    }

   
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
        if (heartPrefab != null)
        {
            Instantiate(heartPrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}