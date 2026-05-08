using UnityEngine;
using UnityEngine.Tilemaps;

public class BerryBushHandler : MonoBehaviour
{
    public TileBase berryBushTile;
    public TileBase normalBushTile;

    public int healAmount = 25;

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Get player's position
        Vector3 playerPos = other.transform.position;

        // Convert to tile position
        Vector3Int tilePos = tilemap.WorldToCell(playerPos);

        // Check if this tile is berry bush
        if (tilemap.GetTile(tilePos) == berryBushTile)
        {
            ReplaceBushAndHeal(tilePos, other.gameObject);
            return;
        }

        // Fallback: Check nearby tiles (very important for walk-through)
        Vector3Int[] nearby =
        {
            tilePos,
            tilePos + Vector3Int.up,
            tilePos + Vector3Int.down,
            tilePos + Vector3Int.left,
            tilePos + Vector3Int.right,
            tilePos + Vector3Int.up + Vector3Int.left,
            tilePos + Vector3Int.up + Vector3Int.right,
            tilePos + Vector3Int.down + Vector3Int.left,
            tilePos + Vector3Int.down + Vector3Int.right
        };

        foreach (Vector3Int pos in nearby)
        {
            if (tilemap.GetTile(pos) == berryBushTile)
            {
                ReplaceBushAndHeal(pos, other.gameObject);
                return;
            }
        }
    }

    private void ReplaceBushAndHeal(Vector3Int tilePos, GameObject player)
    {
        // Heal
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
        }

        // Replace tile
        tilemap.SetTile(tilePos, normalBushTile);
    }
}
