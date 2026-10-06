using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referensi Musuh")]
    public GameObject groundEnemyPrefab;
    public GameObject flyingEnemyPrefab;

    [Header("Titik Kemunculan Ground")]
    public Transform[] groundSpawnPoints;

    [Header("Titik Kemunculan Flying")]
    public Transform[] flyingSpawnPoints;

    [Header("Pengaturan Waktu & Kesulitan")]
    public float initialSpawnInterval = 3f;
    public float minimumSpawnInterval = 0.8f;

    [Header("Batas Jumlah Enemy")]
    public int maxAliveEnemies = 15; // <-- TAMBAHIN INI, batas enemy hidup bersamaan

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
            // Cek dulu jumlah enemy yang masih hidup sebelum spawn baru
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentEnemyCount < maxAliveEnemies)
            {
                SpawnEnemy();
                IncreaseDifficulty();
            }
            // Kalau udah penuh, skip spawn kali ini, tapi timer tetap direset di bawah
            // biar dia terus nyoba lagi di interval berikutnya (bukan macet total)

            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }

    void SpawnEnemy()
    {
        GameObject enemyToSpawn = DecideWhichEnemyToSpawn();
        Transform[] selectedSpawnPoints;

        if (enemyToSpawn == groundEnemyPrefab)
        {
            selectedSpawnPoints = groundSpawnPoints;
        }
        else
        {
            selectedSpawnPoints = flyingSpawnPoints;
        }

        if (selectedSpawnPoints.Length == 0) return;

        Transform randomSpawnPoint =
            selectedSpawnPoints[Random.Range(0, selectedSpawnPoints.Length)];

        Instantiate(enemyToSpawn, randomSpawnPoint.position, Quaternion.identity);
    }

    GameObject DecideWhichEnemyToSpawn()
    {
        if (matchTimer < 30f)
        {
            return groundEnemyPrefab;
        }
        else if (matchTimer < 60f)
        {
            float randomChance = Random.value;
            return (randomChance <= 0.7f) ? groundEnemyPrefab : flyingEnemyPrefab;
        }
        else
        {
            float randomChance = Random.value;
            return (randomChance <= 0.5f) ? groundEnemyPrefab : flyingEnemyPrefab;
        }
    }

    void IncreaseDifficulty()
    {
        if (currentSpawnInterval > minimumSpawnInterval)
        {
            currentSpawnInterval -= 0.05f;
        }
    }
}