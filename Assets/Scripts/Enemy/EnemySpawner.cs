using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnType { Ground, Flying }

    [System.Serializable]
    public class EnemyEntry
    {
        public string name;                 // label biar rapi di Inspector
        public GameObject prefab;
        public SpawnType spawnType = SpawnType.Ground;
        public float startTime = 0f;        // mulai muncul setelah detik ke-berapa
        public float weight = 1f;           // makin besar = makin sering muncul
    }

    [Header("Daftar Enemy")]
    public List<EnemyEntry> enemies = new List<EnemyEntry>();

    [Header("Titik Kemunculan")]
    public Transform[] groundSpawnPoints;
    public Transform[] flyingSpawnPoints;

    [Header("Pengaturan Waktu & Kesulitan")]
    public float initialSpawnInterval = 3f;
    public float minimumSpawnInterval = 0.8f;

    [Header("Batas Jumlah Enemy")]
    public int maxAliveEnemies = 15;

    private float matchTimer = 0f;
    private float currentSpawnInterval;
    private float nextSpawnTime;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        nextSpawnTime = Time.time + currentSpawnInterval;
    }

    void Update()
    {
        matchTimer += Time.deltaTime;

        if (Time.time >= nextSpawnTime)
        {
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentEnemyCount < maxAliveEnemies)
            {
                SpawnEnemy();
                IncreaseDifficulty();
            }

            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }

    void SpawnEnemy()
    {
        EnemyEntry entry = PickEnemy();
        if (entry == null) return;

        Transform[] points = entry.spawnType == SpawnType.Ground ? groundSpawnPoints : flyingSpawnPoints;
        if (points.Length == 0) return;

        Transform point = points[Random.Range(0, points.Length)];
        Instantiate(entry.prefab, point.position, Quaternion.identity);
    }

    // Pilih enemy acak berdasarkan bobot, hanya dari yang sudah "kebuka" (startTime tercapai)
    EnemyEntry PickEnemy()
    {
        float totalWeight = 0f;
        foreach (EnemyEntry e in enemies)
        {
            if (e.prefab != null && matchTimer >= e.startTime)
                totalWeight += e.weight;
        }

        if (totalWeight <= 0f) return null;

        float roll = Random.value * totalWeight;
        foreach (EnemyEntry e in enemies)
        {
            if (e.prefab == null || matchTimer < e.startTime) continue;

            roll -= e.weight;
            if (roll <= 0f) return e;
        }

        return null;
    }

    void IncreaseDifficulty()
    {
        if (currentSpawnInterval > minimumSpawnInterval)
        {
            currentSpawnInterval -= 0.05f;
        }
    }
}