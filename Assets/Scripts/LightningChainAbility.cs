using UnityEngine;
using System.Collections.Generic;

public class LightningChainAbility : MonoBehaviour
{
    private float damage = 25f;
    private int chainJumps = 2;
    private float chainRange = 4f;
    private float cooldown = 2.5f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= cooldown)
        {
            timer = 0f;
            TriggerLightning();
        }
    }

    public void LevelUp(int level)
    {
        damage = 25f + (level - 1) * 15f;
        chainJumps = 1 + level;
        cooldown = Mathf.Max(0.8f, 2.5f - (level - 1) * 0.2f);
    }

    void TriggerLightning()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        // find nearest enemy as first target
        GameObject first = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < minDist) { minDist = d; first = e; }
        }

        if (first == null) return;

        List<GameObject> hit = new List<GameObject>();
        ChainLightning(first, hit, chainJumps, damage);
    }

    void ChainLightning(GameObject target, List<GameObject> alreadyHit, int jumpsLeft, float currentDamage)
    {
        if (target == null) return;

        alreadyHit.Add(target);

        EnemyHealth health = target.GetComponent<EnemyHealth>();
        if (health != null) health.TakeDamage(currentDamage);

        SpawnLightningVisual(target.transform.position);

        if (jumpsLeft <= 0) return;

        // find nearest unchained enemy to jump to
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject next = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            if (alreadyHit.Contains(e)) continue;
            float d = Vector2.Distance(target.transform.position, e.transform.position);
            if (d < chainRange && d < minDist) { minDist = d; next = e; }
        }

        if (next != null)
            ChainLightning(next, alreadyHit, jumpsLeft - 1, currentDamage * 0.7f);
    }

    void SpawnLightningVisual(Vector3 position)
    {
        // simple flash visual
        GameObject flash = new GameObject("LightningFlash");
        flash.transform.position = position;

        SpriteRenderer sr = flash.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.6f, 0.8f, 1f, 0.9f);
        flash.transform.localScale = Vector3.one * 0.6f;

        Destroy(flash, 0.1f);
    }
}