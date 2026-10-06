using UnityEngine;

public class PaintSpawner : MonoBehaviour
{
    [Header("Paint")]
    public GameObject paintPrefab;

    [Header("Ground / Platform")]
    public Collider2D[] platforms;

    [Header("Spawn Settings")]
    public int paintAmount = 10;
    public float spawnInterval = 2f;
    public float heightOffset = 0.5f;
    public float paintLifetime = 8f;

    [Header("Jarak Spawn dari Player")]
    public Transform player;
    public float minDistance = 2f;
    public float maxDistance = 8f;

    [Header("Referensi")]
    public PlayerPaintManager playerPaintManager;

    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;

        if (playerPaintManager == null)
        {
            playerPaintManager = FindObjectOfType<PlayerPaintManager>();
        }
    }

    void Update()
    {
        if (playerPaintManager != null && playerPaintManager.isAttackModeActive)
        {
            nextSpawnTime = Time.time + spawnInterval;
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnPaint();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnPaint()
    {
        if (paintPrefab == null || platforms.Length == 0 || player == null)
            return;

        Vector3 spawnPosition = FindValidSpawnPosition() ?? FallbackPosition();

        GameObject paint = Instantiate(paintPrefab, spawnPosition, Quaternion.identity);

        Destroy(paint, paintLifetime);
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