using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Pre-allocates all enemy instances at startup.
/// The spawner calls Get() / Return() instead of Instantiate / Destroy.
/// Attach this to an empty GameObject called "EnemyPool" in your scene.
/// </summary>
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        [Tooltip("How many instances to create at startup")]
        public int preWarmCount = 60;
    }

    [Header("Pool Entries  —  add one row per enemy prefab")]
    public List<PoolEntry> poolEntries = new List<PoolEntry>();

    private Dictionary<string, Stack<BaseEnemy>> available = new Dictionary<string, Stack<BaseEnemy>>();
    private Dictionary<BaseEnemy, string>         activeKey = new Dictionary<BaseEnemy, string>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var entry in poolEntries)
            PreWarm(entry.prefab, entry.preWarmCount);
    }

    public BaseEnemy Get(GameObject prefab, Vector2 position, float hpMult, float speedMult, float damageMult)
    {
        string key = prefab.name;

        if (!available.ContainsKey(key) || available[key].Count == 0)
            Grow(prefab, 10);

        BaseEnemy enemy = available[key].Pop();
        enemy.transform.position = position;
        enemy.gameObject.SetActive(true);
        activeKey[enemy] = key;
        enemy.OnSpawn(hpMult, speedMult, damageMult);
        return enemy;
    }

    public void Return(BaseEnemy enemy)
    {
        if (enemy == null) return;
        enemy.gameObject.SetActive(false);

        if (activeKey.TryGetValue(enemy, out string key))
        {
            activeKey.Remove(enemy);
            if (!available.ContainsKey(key))
                available[key] = new Stack<BaseEnemy>();
            available[key].Push(enemy);
        }
    }

    public int ActiveCount => activeKey.Count;


    private void PreWarm(GameObject prefab, int count)
    {
        string key = prefab.name;
        if (!available.ContainsKey(key))
            available[key] = new Stack<BaseEnemy>();

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            go.name = key;   

            BaseEnemy enemy = go.GetComponent<BaseEnemy>();
            if (enemy == null)
            {
                Debug.LogError($"[EnemyPool] '{key}' has no BaseEnemy component! Remove it from the pool.");
                Destroy(go);
                continue;
            }

            go.SetActive(false);
            available[key].Push(enemy);
        }
    }

    private void Grow(GameObject prefab, int count)
    {
        Debug.LogWarning($"[EnemyPool] Pool for '{prefab.name}' exhausted — growing by {count}.");
        PreWarm(prefab, count);
    }
}