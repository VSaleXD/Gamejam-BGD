using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerLavaCheck : MonoBehaviour
{
    [Header("Tilemap Check (Opsional)")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase brokenTile;

    [Header("Fall Zone Check (Layer)")]
    [SerializeField] private LayerMask fallLayers;

    [Header("Prefab Broken Tile Check")]
    [SerializeField] private bool checkBrokenFloorRetak = true;
    [SerializeField] private float brokenTileCheckRadius = 0.2f;

    private game gameManager;
    private bool alreadyDead;
    private bool warnedNoFallLayer;

    private void Awake()
    {
        gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();
    }

    private void Update()
    {
        if (alreadyDead)
        {
            return;
        }

        if (tilemap != null && brokenTile != null)
        {
            Vector3 worldPos = transform.position;

            Vector3Int cellPos = tilemap.WorldToCell(worldPos);

            TileBase currentTile = tilemap.GetTile(cellPos);

            if (currentTile == brokenTile)
            {
                TriggerGameOver("Player jatuh ke broken tile.");
                return;
            }
        }

        if (checkBrokenFloorRetak)
        {
            CheckBrokenPrefabTileUnderPlayer();
        }
    }

    private void CheckBrokenPrefabTileUnderPlayer()
    {
        Collider2D[] nearbyHits = Physics2D.OverlapCircleAll(transform.position, brokenTileCheckRadius);
        for (int i = 0; i < nearbyHits.Length; i++)
        {
            Collider2D hit = nearbyHits[i];
            if (hit == null)
            {
                continue;
            }

            floorRetak tile = hit.GetComponent<floorRetak>();
            if (tile == null)
            {
                tile = hit.GetComponentInParent<floorRetak>();
            }

            if (tile != null && tile.IsBroken)
            {
                TriggerGameOver("Player berdiri di tile broken (floorRetak).");
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleFallCollision(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        HandleFallCollision(other.gameObject);
    }

    private void HandleFallCollision(GameObject other)
    {
        if (alreadyDead)
        {
            return;
        }

        floorRetak tile = other.GetComponent<floorRetak>();
        if (tile == null)
        {
            tile = other.GetComponentInParent<floorRetak>();
        }

        if (tile != null)
        {
            if (tile.IsBroken)
            {
                TriggerGameOver("Player menyentuh tile broken.");
            }

            return;
        }

        if (fallLayers.value == 0)
        {
            if (!warnedNoFallLayer)
            {
                warnedNoFallLayer = true;
                Debug.LogWarning("PlayerLavaCheck: fallLayers belum di-set. Set layer area jatuh di inspector.");
            }

            return;
        }

        int otherLayerMask = 1 << other.layer;
        if ((fallLayers.value & otherLayerMask) != 0)
        {
            TriggerGameOver("Player menyentuh layer FALL.");
        }
    }

    private void TriggerGameOver(string reason)
    {
        alreadyDead = true;

        if (gameManager != null)
        {
            gameManager.LoseRun();
        }

        Debug.Log(reason);
    }

    private void OnDrawGizmosSelected()
    {
        if (!checkBrokenFloorRetak)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, brokenTileCheckRadius);
    }
}