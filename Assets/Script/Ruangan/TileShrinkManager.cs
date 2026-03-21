using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class TileShrinkManager : MonoBehaviour
{
    [Header("Tiles")]
    public Tilemap tilemap;
    public TileBase normalTile;
    public TileBase crack1Tile;
    public TileBase crack2Tile;
    public TileBase brokenTile;

    [Header("Grid")]
    public int width = 36;
    public int height = 20;

    [Header("Timing")]
    public float crackDelay = 1f;
    public float breakDelay = 0.4f;
    public float startDelay = 2f;

    [Header("Difficulty Scaling")]
    [SerializeField] private float delayMultiplierPerRound = 0.93f;
    [SerializeField] private float minDelayMultiplier = 0.35f;
    [SerializeField] private bool runOnStart;

    private float baseCrackDelay;
    private float baseBreakDelay;
    private float baseStartDelay;

    private void Awake()
    {
        baseCrackDelay = crackDelay;
        baseBreakDelay = breakDelay;
        baseStartDelay = startDelay;
    }

    private void Start()
    {
        if (tilemap == null)
        {
            Debug.LogWarning("TileShrinkManager: tilemap belum di-assign.");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        width = bounds.size.x;
        height = bounds.size.y;

        if (runOnStart)
        {
            BeginPressure();
        }
    }

    private Coroutine shrinkRoutine;

    public void BeginPressure()
    {
        if (shrinkRoutine == null)
        {
            shrinkRoutine = StartCoroutine(ShrinkRoutine());
        }
    }

    public void PrepareForRound(int roundNumber)
    {
        StopPressure();
        ResetToNormal();
        ApplyDifficulty(roundNumber);
        BeginPressure();
    }

    public void StopPressure()
    {
        if (shrinkRoutine != null)
        {
            StopCoroutine(shrinkRoutine);
            shrinkRoutine = null;
        }
    }

    int GetLayer(int x, int y)
    {
        return Mathf.Min(
            x,
            y,
            width - 1 - x,
            height - 1 - y
        );
    }
    
    void SetLayerTile(int targetLayer, TileBase tile)
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                int localX = x - bounds.xMin;
                int localY = y - bounds.yMin;

                if (GetLayer(localX, localY) == targetLayer)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    tilemap.SetTile(pos, tile);
                }
            }
        }
    }

    IEnumerator ShrinkRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        int maxLayer = Mathf.Min(width, height) / 2;

        for (int layer = 0; layer < maxLayer; layer++)
        {
            SetLayerTile(layer, crack1Tile);
            yield return new WaitForSeconds(crackDelay);

            SetLayerTile(layer, crack2Tile);
            yield return new WaitForSeconds(crackDelay);

            SetLayerTile(layer, brokenTile);
            yield return new WaitForSeconds(breakDelay);
        }

        shrinkRoutine = null;
    }

    private void ResetToNormal()
    {
        if (tilemap == null || normalTile == null)
        {
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(pos) != null)
                {
                    tilemap.SetTile(pos, normalTile);
                }
            }
        }
    }

    private void ApplyDifficulty(int roundNumber)
    {
        int safeRound = Mathf.Max(1, roundNumber);
        float delayScale = Mathf.Pow(delayMultiplierPerRound, safeRound - 1);
        delayScale = Mathf.Max(minDelayMultiplier, delayScale);

        crackDelay = baseCrackDelay * delayScale;
        breakDelay = baseBreakDelay * delayScale;
        startDelay = baseStartDelay * delayScale;
    }
}

