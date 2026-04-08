using UnityEngine;
using System.Collections.Generic;

public class LightningChainAbility : MonoBehaviour
{
    private float damage     = 25f;
    private int   chainJumps = 2;
    private float chainRange = 4f;
    private float cooldown   = 2.5f;
    private float timer      = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= cooldown) { timer = 0f; TriggerLightning(); }
    }

    public void LevelUp(int level)
    {
        damage     = 25f + (level - 1) * 15f;
        chainJumps = 1 + level;
        cooldown   = Mathf.Max(0.8f, 2.5f - (level - 1) * 0.2f);
    }

    void TriggerLightning()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        GameObject first   = null;
        float      minDist = Mathf.Infinity;
        foreach (GameObject e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < minDist) { minDist = d; first = e; }
        }

        if (first == null) return;
        ChainLightning(first, new List<GameObject>(), chainJumps, damage, transform.position);
    }

    void ChainLightning(GameObject target, List<GameObject> alreadyHit, int jumpsLeft, float currentDamage, Vector3 fromPos)
    {
        if (target == null) return;

        alreadyHit.Add(target);

        BaseEnemy enemy = target.GetComponent<BaseEnemy>();
        if (enemy != null) enemy.TakeDamage(currentDamage);

        SpawnLightningVisual(fromPos, target.transform.position);

        if (jumpsLeft <= 0) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject   next    = null;
        float        minDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            if (alreadyHit.Contains(e)) continue;
            float d = Vector2.Distance(target.transform.position, e.transform.position);
            if (d < chainRange && d < minDist) { minDist = d; next = e; }
        }

        if (next != null)
            ChainLightning(next, alreadyHit, jumpsLeft - 1, currentDamage * 0.7f, target.transform.position);
    }

    void SpawnLightningVisual(Vector3 fromPos, Vector3 toPos)
    {
        GameObject flash = new GameObject("LightningFlash");
        Vector3    diff  = toPos - fromPos;
        float      dist  = diff.magnitude;
        float      angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        flash.transform.position   = fromPos + diff * 0.5f;
        flash.transform.rotation   = Quaternion.AngleAxis(angle, Vector3.forward);
        flash.transform.localScale = new Vector3(dist, 0.15f, 1f);

        SpriteRenderer sr = flash.AddComponent<SpriteRenderer>();
        sr.sprite       = SpriteHelper.CreateSquare();
        sr.color        = new Color(0.6f, 0.8f, 1f, 0.9f);
        sr.sortingOrder = 2;

        Destroy(flash, 0.12f);
    }
}