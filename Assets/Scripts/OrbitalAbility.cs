using UnityEngine;
using System.Collections.Generic;

public class OrbitalAbility : MonoBehaviour
{
    private int orbCount = 1;
    private float orbitRadius = 2f;
    private float orbitSpeed = 180f;
    private float damage = 15f;
    private float damageCooldown = 0.5f;
    private List<GameObject> orbs = new List<GameObject>();
    private float angle = 0f;

    public void LevelUp(int level)
    {
        orbCount = level;
        damage = 15f + (level - 1) * 10f;
        orbitRadius = 2f + (level - 1) * 0.2f;
        RefreshOrbs();
    }

    void Update()
    {
        angle += orbitSpeed * Time.deltaTime;
        if (angle >= 360f) angle -= 360f;

        for (int i = 0; i < orbs.Count; i++)
        {
            if (orbs[i] == null) continue;
            float orbAngle = angle + (360f / orbs.Count) * i;
            float rad = orbAngle * Mathf.Deg2Rad;
            orbs[i].transform.position = transform.position + new Vector3(
                Mathf.Cos(rad) * orbitRadius,
                Mathf.Sin(rad) * orbitRadius,
                0f
            );
        }
    }

    void RefreshOrbs()
    {
        foreach (GameObject orb in orbs)
            if (orb != null) Destroy(orb);
        orbs.Clear();

        for (int i = 0; i < orbCount; i++)
            orbs.Add(CreateOrbVisual());
    }

    GameObject CreateOrbVisual()
    {
        GameObject orb = new GameObject("Orb");

        SpriteRenderer sr = orb.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.CreateCircle();
        sr.color = new Color(1f, 0.6f, 0f, 1f);
        sr.sortingOrder = 2;
        orb.transform.localScale = Vector3.one * 0.4f;

        CircleCollider2D col = orb.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        OrbDamager damager = orb.AddComponent<OrbDamager>();
        damager.damage = damage;
        damager.damageCooldown = damageCooldown;

        return orb;
    }
}