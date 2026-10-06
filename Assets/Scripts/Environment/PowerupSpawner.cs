using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    [Header("Referensi")]
    public GameObject powerupPrefab;
    public Transform player;

    [Header("Ground / Platform")]
    public Collider2D[] platforms;

    [Header("Jarak Spawn dari Player")]
    public float minDistance = 3f;
    public float maxDistance = 6f;
    public float heightOffset = 0.5f;

    [Header("Pengaturan")]
    public int paintPerPowerup = 10;

    private int paintCollectedCount = 0;
    private bool powerupPending = false; // true kalau syarat 10 paint udah kepenuhi, tinggal nunggu slot kosong
    private GameObject activePowerup;

    void OnEnable() => PaintCollectible.OnPaintCollected += HandlePaintCollected;
    void OnDisable() => PaintCollectible.OnPaintCollected -= HandlePaintCollected;

    void HandlePaintCollected()
    {
        paintCollectedCount++;

        if (paintCollectedCount >= paintPerPowerup)
        {
            // Reset counter LANGSUNG begitu nyampe threshold,
            // gak peduli spawn-nya kejadian sekarang atau nanti
            paintCollectedCount = 0;
            powerupPending = true;
        }

        TrySpawnPending();
    }

    void TrySpawnPending()
    {
        if (!powerupPending) return;

        bool powerupStillExists = activePowerup != null && activePowerup.activeInHierarchy;

        if (!powerupStillExists)
        {
            powerupPending = false;
            SpawnPowerup();
        }
        // Kalau masih ada powerup lama yang belum diambil, tetap nunggu (powerupPending tetap true)
        // sampai player ambil powerup itu dulu
    }

    void SpawnPowerup()
    {
        if (powerupPrefab == null || player == null || platforms.Length == 0)
            return;

        Vector3 spawnPos = FindValidSpawnPosition() ?? FallbackPosition();

        activePowerup = Instantiate(powerupPrefab, spawnPos, Quaternion.identity);
    }

    Vector3? FindValidSpawnPosition()
    {
        const int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            Collider2D platform = platforms[Random.Range(0, platforms.Length)];
            Bounds bounds = platform.bounds;

            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float spawnY = bounds.max.y + heightOffset;
            Vector3 candidate = new Vector3(randomX, spawnY, 0f);

            float dist = Vector2.Distance(candidate, player.position);

            if (dist >= minDistance && dist <= maxDistance)
            {
                return candidate;
            }
        }

        return null;
    }

    Vector3 FallbackPosition()
    {
        Collider2D platform = platforms[Random.Range(0, platforms.Length)];
        Bounds bounds = platform.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float spawnY = bounds.max.y + heightOffset;
        return new Vector3(randomX, spawnY, 0f);
    }
}