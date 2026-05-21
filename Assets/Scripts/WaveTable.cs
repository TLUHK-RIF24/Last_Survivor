using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveEnemyEntry
{
    [Tooltip("Enemy prefab for this type")]
    public GameObject prefab;

    [Tooltip("Minimum alive count. Spawner fills to this quota every tick.")]
    public int minimumAlive = 10;
}

[System.Serializable]
public class WaveDefinition
{
    [Tooltip("Human-readable label — no effect on gameplay")]
    public string label = "Minute 0";

    [Tooltip("Enemy types active this minute and their minimum alive quotas")]
    public List<WaveEnemyEntry> enemies = new List<WaveEnemyEntry>();

    [Tooltip("How often (seconds) the spawner checks and fills quotas")]
    public float spawnInterval = 1.0f;
}

public enum MapEventType
{
    SwarmRush,    // flood from one screen edge
    WallSpawn,    // ring around the player
    ArenaRing,    // closing ring
}

[System.Serializable]
public class MapEvent
{
    public string       label        = "Event";
    public MapEventType eventType    = MapEventType.SwarmRush;

    [Tooltip("Seconds into the run when this fires (e.g. 150 = minute 2 second 30)")]
    public float        triggerTime  = 60f;

    public GameObject   enemyPrefab;
    public int          enemyCount   = 30;
    public float        ringRadius   = 7f;

    [Tooltip("Duration in seconds (only used for SwarmRush)")]
    public float        duration     = 20f;

    [HideInInspector] public bool triggered = false;
}

[CreateAssetMenu(fileName = "WaveTable", menuName = "Enemies/Wave Table")]
public class WaveTable : ScriptableObject
{
    [Header("Hard Cap")]
    [Tooltip("No regular spawns above this count — mirrors the VS 300 cap")]
    public int hardCap = 300;

    [Header("Despawn / Teleport")]
    [Tooltip("Enemies beyond this distance from the player get teleported to the spawn ring")]
    public float despawnDistance      = 22f;
    public float teleportScanInterval = 2f;

    [Header("Wave Schedule  (one entry per minute, index 0 = minute 0)")]
    public List<WaveDefinition> waves = new List<WaveDefinition>();

    [Header("Map Events")]
    public List<MapEvent> mapEvents = new List<MapEvent>();

    public WaveDefinition GetWave(float gameTime)
    {
        if (waves == null || waves.Count == 0) return null;
        int index = Mathf.FloorToInt(gameTime / 60f);
        index = Mathf.Clamp(index, 0, waves.Count - 1);
        return waves[index];
    }

    public int GetWaveIndex(float gameTime)
        => Mathf.Clamp(Mathf.FloorToInt(gameTime / 60f), 0, Mathf.Max(0, waves.Count - 1));
}