using UnityEngine;
using System.Collections.Generic;

// ═════════════════════════════════════════════════════════════════════════════
//  SpawnRule  —  one enemy type inside a MinuteWindow
//  (replaces the old standalone SpawnRule.cs ScriptableObject)
// ═════════════════════════════════════════════════════════════════════════════
[System.Serializable]
public class SpawnRule
{
    [Tooltip("Enemy prefab — must have a BaseEnemy subclass on it")]
    public GameObject enemyPrefab;

    [Range(0f, 100f)]
    [Tooltip("Higher = more likely to be picked within this window")]
    public float spawnWeight = 10f;

    [Tooltip("Fire several at once instead of one at a time")]
    public bool  spawnInClusters = false;
    public int   minClusterSize  = 3;
    public int   maxClusterSize  = 8;
    public float clusterSpread   = 1.5f;
}

// ═════════════════════════════════════════════════════════════════════════════
//  MinuteWindow  —  which enemies are active during a time band
// ═════════════════════════════════════════════════════════════════════════════
[System.Serializable]
public class MinuteWindow
{
    [Tooltip("Window becomes active at this many seconds into the run")]
    public float startTime = 0f;

    [Tooltip("Window ends at this many seconds. Set to 0 to run until the end")]
    public float endTime   = 60f;

    [Tooltip("Label shown in the Inspector only — has no effect on gameplay")]
    public string label    = "Minute 1";

    public List<SpawnRule> rules = new List<SpawnRule>();
}

// ═════════════════════════════════════════════════════════════════════════════
//  SpecialEvent  —  timed interventions that break the normal spawn loop
// ═════════════════════════════════════════════════════════════════════════════
public enum SpecialEventType
{
    SwarmRush,   // continuous flood from one screen edge for N seconds
    ArenaRing,   // ring of enemies spawns around the player instantly
    Stalker,     // one very tanky enemy that never stops following the player
    WallSpawn,   // ring of tough enemies — the DPS counter-measure
}

[System.Serializable]
public class SpecialEvent
{
    public SpecialEventType eventType   = SpecialEventType.SwarmRush;
    public float            triggerTime = 120f;
    public float            duration    = 30f;
    public GameObject       enemyPrefab;
    public int              enemyCount  = 40;
    public float            ringRadius  = 6f;
    [HideInInspector] public bool triggered = false;
}

[CreateAssetMenu(fileName = "WaveTable", menuName = "Enemies/Wave Table")]
public class WaveTable : ScriptableObject
{
    [Header("Enemy Cap  (how many can be alive at once)")]
    public int   startEnemyCap = 60;
    public int   maxEnemyCap   = 500;
    [Tooltip("Seconds to grow from startEnemyCap to maxEnemyCap")]
    public float capRampTime   = 900f;

    [Header("Spawn Pulse Speed")]
    [Tooltip("Seconds between spawn pulses at the very start")]
    public float baseSpawnInterval = 1.0f;
    [Tooltip("Fastest the pulse can ever get")]
    public float minSpawnInterval  = 0.15f;
    [Tooltip("Seconds for the pulse to reach its fastest")]
    public float intervalRampTime  = 600f;

    [Header("Stat Scaling  (enemies get tougher over time)")]
    [Tooltip("HP doubles after this many seconds")]
    public float hpScaleTime      = 600f;
    [Tooltip("How much speed grows (fraction of base per scaleTime)")]
    public float speedScaleFactor = 0.3f;
    public float speedScaleTime   = 600f;
    [Tooltip("Damage doubles after this many seconds")]
    public float damageScaleTime  = 480f;

    [Header("DPS Wall  (fires when the player kills too fast)")]
    [Tooltip("Kills per second needed to trigger a wall spawn")]
    public float wallTriggerKillRate = 8f;
    [Tooltip("Minimum seconds between wall spawns")]
    public float wallSpawnCooldown   = 20f;

    [Header("Off-Screen Recycling")]
    [Tooltip("Enemies beyond this distance from the player get teleported to the spawn ring")]
    public float despawnDistance      = 22f;
    [Tooltip("Seconds between teleport scans (lower = more accurate but more CPU)")]
    public float teleportScanInterval = 2f;

    [Header("Minute Windows")]
    public List<MinuteWindow> windows = new List<MinuteWindow>();

    [Header("Special Events")]
    public List<SpecialEvent> specialEvents = new List<SpecialEvent>();

    public int GetEnemyCap(float t)
    {
        float pct = Mathf.Clamp01(t / capRampTime);
        return Mathf.RoundToInt(Mathf.Lerp(startEnemyCap, maxEnemyCap, pct));
    }

    public float GetSpawnInterval(float t)
    {
        float pct = Mathf.Clamp01(t / intervalRampTime);
        return Mathf.Lerp(baseSpawnInterval, minSpawnInterval, pct);
    }

    public float GetHPMultiplier(float t)     => 1f + (t / hpScaleTime);
    public float GetSpeedMultiplier(float t)  => 1f + (t / speedScaleTime) * speedScaleFactor;
    public float GetDamageMultiplier(float t) => 1f + (t / damageScaleTime);

    public MinuteWindow GetActiveWindow(float t)
    {
        MinuteWindow best = null;
        foreach (var w in windows)
            if (t >= w.startTime && (w.endTime <= 0f || t < w.endTime))
                best = w;
        return best;
    }
}