using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerLavaCheck : MonoBehaviour
{
    [Header("Tilemap Check (Opsional)")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase brokenTile;

    [Header("Fall Zone Check")]
    [SerializeField] private bool useFallTag = true;
    [SerializeField] private string fallTag = "FALL";
    [SerializeField] private LayerMask fallLayers;

    private game gameManager;
    private bool alreadyDead;

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

        if (tilemap == null || brokenTile == null)
        {
            return;
        }

        Vector3 worldPos = transform.position;

        Vector3Int cellPos = tilemap.WorldToCell(worldPos);

        TileBase currentTile = tilemap.GetTile(cellPos);

        if (currentTile == brokenTile)
        {
            TriggerGameOver("Player jatuh ke broken tile.");
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

        if (useFallTag && other.CompareTag(fallTag))
        {
            TriggerGameOver("Player menyentuh area FALL.");
            return;
        }

        if (fallLayers.value != 0)
        {
            int otherLayerMask = 1 << other.layer;
            if ((fallLayers.value & otherLayerMask) != 0)
            {
                TriggerGameOver("Player menyentuh layer FALL.");
            }
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
}