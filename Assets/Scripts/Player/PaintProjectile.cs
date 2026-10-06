using UnityEngine;

public class PaintProjectile : MonoBehaviour
{
    [Header("Gerak")]
    public float speed = 12f;
    public float maxDistance = 8f;
    public float arcHeight = 0.4f;

    [Header("Serangan")]
    public int damage = 1;

    [Header("Efek")]
    public GameObject splashEffectPrefab;

    private Vector3 startPosition;
    private Vector2 direction = Vector2.right;
    private float traveledDistance = 0f;
    private bool hasHit = false;

    [Header("Knockback")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.2f;
    public float knockbackUpwardBias = 0.3f;

    void Start()
    {
        startPosition = transform.position;
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        traveledDistance += step;

        // Posisi dasar: gerak lurus ke arah manapun (bukan cuma horizontal lagi)
        Vector3 basePosition = startPosition + (Vector3)(direction * traveledDistance);

        float progress = Mathf.Clamp01(traveledDistance / maxDistance);

        // Lengkungan tetap ditambahkan di sumbu Y dunia (efek "gravitasi ringan"),
        // ini tetap masuk akal buat arah manapun -> selalu melengkung dikit ke atas lalu turun
        float verticalOffset = Mathf.Sin(progress * Mathf.PI) * arcHeight;

        transform.position = basePosition + new Vector3(0, verticalOffset, 0);

        // Hitung rotasi visual berdasarkan arah dasar + kemiringan lengkungan saat ini,
        // sekarang pakai direction penuh (x DAN y), bukan cuma horizontal
        float tangentSlope = Mathf.Cos(progress * Mathf.PI) * arcHeight * (Mathf.PI / Mathf.Max(maxDistance, 0.01f));
        Vector2 tangent = direction + new Vector2(0, tangentSlope);
        float visualAngle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, visualAngle);

        if (traveledDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Enemy"))
        {
            hasHit = true;

            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                Vector2 knockbackDir = new Vector2(direction.x, knockbackUpwardBias).normalized;
                enemy.ApplyKnockback(knockbackDir, knockbackForce, knockbackDuration);
            }

            SpawnSplash();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall") || collision.CompareTag("Platform"))
        {
            hasHit = true;
            SpawnSplash();
            Destroy(gameObject);
        }
    }

    void SpawnSplash()
    {
        if (splashEffectPrefab != null)
        {
            Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}