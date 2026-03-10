using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnRule", menuName = "Enemies/Spawn Rule")]
public class SpawnRule : ScriptableObject
{
    public GameObject enemyPrefab;
    public float startTime = 0f;
    public float spawnWeight = 1f;
    public bool spawnInClusters = false;
    public int minClusterSize = 3;
    public int maxClusterSize = 8;
    public float clusterSpread = 1.5f;
}