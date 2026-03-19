using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Rules")]
    public List<SpawnRule> spawnRules;

    [Header("Spawner Settings")]
    public float baseSpawnInterval = 1f;
    public float minSpawnInterval = 0.3f;
    public float difficultyRampInterval = 30f;

    private float spawnTimer = 0f;
    private float currentSpawnInterval;
    private float gameTime = 0f;
    private Transform player;
    private Camera cam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main;
        currentSpawnInterval = baseSpawnInterval;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        currentSpawnInterval = Mathf.Max(
            minSpawnInterval,
            baseSpawnInterval - (gameTime / difficultyRampInterval) * 0.3f
        );

        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        SpawnRule rule = PickRule();
        if (rule == null) return;

        if (rule.spawnInClusters)
        {
            SpawnCluster(rule);
        }
        else
        {
            SpawnSingle(rule);
        }
    }

    void SpawnSingle(SpawnRule rule)
    {
        Vector2 spawnPos = GetScreenEdgePosition();
        Instantiate(rule.enemyPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnCluster(SpawnRule rule)
    {
        Vector2 edgePoint = GetScreenEdgePosition();
        int clusterSize = Random.Range(rule.minClusterSize, rule.maxClusterSize + 1);

        for (int i = 0; i < clusterSize; i++)
        {
            Vector2 scatter = Random.insideUnitCircle * rule.clusterSpread;
            Vector2 spawnPos = edgePoint + scatter;
            Instantiate(rule.enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    SpawnRule PickRule()
    {
        List<SpawnRule> activeRules = new List<SpawnRule>();
        float totalWeight = 0f;

        foreach (SpawnRule rule in spawnRules)
        {
            if (gameTime >= rule.startTime)
            {
                activeRules.Add(rule);
                totalWeight += rule.spawnWeight;
            }
        }

        if (activeRules.Count == 0) return null;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (SpawnRule rule in activeRules)
        {
            cumulative += rule.spawnWeight;
            if (roll <= cumulative)
                return rule;
        }

        return activeRules[activeRules.Count - 1];
    }

    Vector2 GetScreenEdgePosition()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        float padding = 1.5f;

        Vector2 playerPos = player.position;
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: // top
                return new Vector2(
                    playerPos.x + Random.Range(-camWidth, camWidth),
                    playerPos.y + camHeight + padding
                );
            case 1: // bottom
                return new Vector2(
                    playerPos.x + Random.Range(-camWidth, camWidth),
                    playerPos.y - camHeight - padding
                );
            case 2: // left
                return new Vector2(
                    playerPos.x - camWidth - padding,
                    playerPos.y + Random.Range(-camHeight, camHeight)
                );
            default: // right
                return new Vector2(
                    playerPos.x + camWidth + padding,
                    playerPos.y + Random.Range(-camHeight, camHeight)
                );
        }
    }
}