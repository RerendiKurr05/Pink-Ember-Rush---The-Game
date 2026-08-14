using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referensi Musuh")]
    public GameObject groundEnemyPrefab;
    public GameObject flyingEnemyPrefab;
    // Nanti bisa tambahkan dasher/shooter di sini

    [Header("Titik Kemunculan (Spawners)")]
    public Transform[] spawnPoints;

    [Header("Pengaturan Waktu & Kesulitan")]
    public float initialSpawnInterval = 3f;
    public float minimumSpawnInterval = 0.8f;
    
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

        // Jika waktunya memunculkan musuh
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            
            nextSpawnTime = Time.time + currentSpawnInterval;
            
            IncreaseDifficulty();
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemyToSpawn = DecideWhichEnemyToSpawn();

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
            float randomChance = Random.value; // Nilai 0.0 sampai 1.0
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