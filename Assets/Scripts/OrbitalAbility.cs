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
        {
            GameObject orb = CreateOrbVisual();
            orbs.Add(orb);
        }
    }

    GameObject CreateOrbVisual()
    {
        GameObject orb = new GameObject("Orb");
        orb.tag = "Untagged";

        SpriteRenderer sr = orb.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = new Color(1f, 0.6f, 0f, 1f); 
        orb.transform.localScale = Vector3.one * 0.4f;

        CircleCollider2D col = orb.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        OrbDamager damager = orb.AddComponent<OrbDamager>();
        damager.damage = damage;
        damager.damageCooldown = damageCooldown;

        return orb;
    }

    Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        float r = size / 2;

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                tex.SetPixel(x, y, dist <= r ? Color.white : Color.clear);
            }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}