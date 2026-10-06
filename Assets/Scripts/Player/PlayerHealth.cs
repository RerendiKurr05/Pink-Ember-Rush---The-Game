using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Referensi")]
    public GameManager gameManager;

    [Header("Statistik")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("UI")]
    public Image healthBarFill;
    public Animator animator;

    [Header("Invincibility (kebal sesaat setelah kena damage)")]
    public float invincibilityDuration = 1f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    [Header("Efek Visual (opsional)")]
    public SpriteRenderer sr;
    public float flashInterval = 0.1f;
    private float flashTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;

            // Efek kedap-kedip pas lagi kebal, biar keliatan jelas kalau lagi invincible
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f && sr != null)
            {
                sr.enabled = !sr.enabled;
                flashTimer = flashInterval;
            }

            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                if (sr != null) sr.enabled = true; // pastikan sprite balik keliatan normal
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // lagi kebal, damage diabaikan

        Debug.Log("PLAYER KENA HIT!");

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthBar();

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            flashTimer = flashInterval;
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Pemain Kalah!");

        if (gameManager != null)
        {
            gameManager.GameOver();
        }

        gameObject.SetActive(false);
    }
}