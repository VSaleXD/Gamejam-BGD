using System.Collections.Generic;
using UnityEngine;

public class FloorPressureManager : MonoBehaviour
{
    private enum PressureMode
    {
        Random,
        EdgeToCenter
    }

    [Header("Pressure Timing")]
    [SerializeField] private bool runOnStart = true;
    [SerializeField] private float initialDelay = 2f;
    [SerializeField] private float waveInterval = 2f;
    [SerializeField] private int cracksPerWave = 1;

    [Header("Difficulty Scaling")]
    [SerializeField] private float waveIntervalMultiplierPerRound = 0.92f;
    [SerializeField] private float minWaveInterval = 0.4f;
    [SerializeField] private float crackDelayMultiplierPerRound = 0.93f;
    [SerializeField] private float minCrackDelayMultiplier = 0.4f;

    [Header("Pressure Pattern")]
    [SerializeField] private PressureMode pressureMode = PressureMode.EdgeToCenter;
    [SerializeField, Range(0f, 0.5f)] private float edgeBandWidth = 0.08f;
    [SerializeField] private Transform centerPoint;

    [Header("Tile Source")]
    [SerializeField] private bool autoFindTilesFromChildren = true;
    [SerializeField] private Transform tilesRoot;
    [SerializeField] private floorRetak[] tiles;

    [Header("Tilemap Mode")]
    [SerializeField] private bool useTileShrinkManager;
    [SerializeField] private TileShrinkManager tileShrinkManager;

    [Header("Random")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int fixedSeed = 12345;

    private float timer;
    private bool started;
    private float baseWaveInterval;
    private float currentCrackDelayScale = 1f;

    public float CurrentWaveInterval => GetCurrentWaveInterval();
    public float CurrentCrackDelayScale => currentCrackDelayScale;
    public float BaseWaveInterval => Mathf.Max(0.0001f, baseWaveInterval);
    public Transform TilesRoot => tilesRoot;

    private void Awake()
    {
        baseWaveInterval = Mathf.Max(0.0001f, waveInterval);
    }

    private void Start()
    {
        SetupRandomSeed();

        if (!useTileShrinkManager)
        {
            RefreshTiles();
        }

        timer = initialDelay;
        started = runOnStart && !useTileShrinkManager;

        if (useTileShrinkManager && runOnStart && tileShrinkManager != null)
        {
            tileShrinkManager.PrepareForRound(1);
        }
    }

    private void Update()
    {
        if (!started)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer > 0f)
        {
            return;
        }

        TriggerPressureWave();
        timer = GetCurrentWaveInterval();
    }

    public void PrepareForRound(int roundNumber)
    {
        if (useTileShrinkManager)
        {
            if (tileShrinkManager != null)
            {
                tileShrinkManager.PrepareForRound(roundNumber);
            }
            else
            {
                Debug.LogWarning("FloorPressureManager: useTileShrinkManager aktif tapi reference TileShrinkManager kosong.");
            }

            return;
        }

        RefreshTiles();
        ResetAllTilesToSafe();
        ApplyDifficulty(roundNumber);
        timer = initialDelay;
        started = runOnStart;
    }

    public void StartPressure()
    {
        if (useTileShrinkManager)
        {
            if (tileShrinkManager != null)
            {
                tileShrinkManager.BeginPressure();
            }

            return;
        }

        started = true;
        timer = initialDelay;
    }

    public void StopPressure()
    {
        if (useTileShrinkManager)
        {
            if (tileShrinkManager != null)
            {
                tileShrinkManager.StopPressure();
            }

            return;
        }

        started = false;
    }

    public void RefreshTiles()
    {
        if (useTileShrinkManager)
        {
            return;
        }

        if (autoFindTilesFromChildren)
        {
            Transform sourceRoot = tilesRoot != null ? tilesRoot : transform;
            tiles = sourceRoot.GetComponentsInChildren<floorRetak>(true);
        }

        if (tiles == null || tiles.Length == 0)
        {
            Debug.LogWarning("FloorPressureManager: tile floorRetak belum ditemukan.");
        }
    }

    public void SetTilesRoot(Transform newTilesRoot, bool refreshNow)
    {
        if (useTileShrinkManager)
        {
            return;
        }

        tilesRoot = newTilesRoot;

        if (refreshNow)
        {
            RefreshTiles();
        }
    }

    public void ResetAllTilesToSafe()
    {
        if (useTileShrinkManager)
        {
            return;
        }

        if (tiles == null)
        {
            return;
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                tiles[i].ForceResetToSafe();
            }
        }
    }

    public void TriggerPressureWave()
    {
        if (useTileShrinkManager)
        {
            return;
        }

        if (tiles == null || tiles.Length == 0)
        {
            return;
        }

        List<floorRetak> safeTiles = new List<floorRetak>();

        for (int i = 0; i < tiles.Length; i++)
        {
            floorRetak tile = tiles[i];
            if (tile != null && tile.IsSafe)
            {
                safeTiles.Add(tile);
            }
        }

        if (safeTiles.Count == 0)
        {
            return;
        }

        int picks = Mathf.Clamp(cracksPerWave, 1, safeTiles.Count);

        if (pressureMode == PressureMode.Random)
        {
            CrackRandomTiles(safeTiles, picks);
            return;
        }

        CrackFromEdgeToCenter(safeTiles, picks);
    }

    private void CrackRandomTiles(List<floorRetak> safeTiles, int picks)
    {
        for (int i = 0; i < picks; i++)
        {
            int index = Random.Range(0, safeTiles.Count);
            floorRetak selectedTile = safeTiles[index];
            safeTiles.RemoveAt(index);

            selectedTile.TriggerCrack();
        }
    }

    private void CrackFromEdgeToCenter(List<floorRetak> safeTiles, int picks)
    {
        Vector2 center = GetCenterPoint();
        GetHalfExtents(center, out float halfX, out float halfY);

        for (int i = 0; i < picks; i++)
        {
            if (safeTiles.Count == 0)
            {
                return;
            }

            float bestScore = float.MinValue;

            for (int j = 0; j < safeTiles.Count; j++)
            {
                float score = GetEdgeScore(safeTiles[j].transform.position, center, halfX, halfY);
                if (score > bestScore)
                {
                    bestScore = score;
                }
            }

            float threshold = bestScore - edgeBandWidth;
            List<int> candidateIndices = new List<int>();

            for (int j = 0; j < safeTiles.Count; j++)
            {
                float score = GetEdgeScore(safeTiles[j].transform.position, center, halfX, halfY);
                if (score >= threshold)
                {
                    candidateIndices.Add(j);
                }
            }

            int pickedListIndex = candidateIndices[Random.Range(0, candidateIndices.Count)];
            floorRetak selectedTile = safeTiles[pickedListIndex];
            safeTiles.RemoveAt(pickedListIndex);

            selectedTile.TriggerCrack();
        }
    }

    private Vector2 GetCenterPoint()
    {
        if (centerPoint != null)
        {
            return centerPoint.position;
        }

        Vector2 sum = Vector2.zero;
        int count = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                continue;
            }

            sum += (Vector2)tiles[i].transform.position;
            count++;
        }

        if (count == 0)
        {
            return Vector2.zero;
        }

        return sum / count;
    }

    private void GetHalfExtents(Vector2 center, out float halfX, out float halfY)
    {
        halfX = 0f;
        halfY = 0f;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                continue;
            }

            Vector2 pos = tiles[i].transform.position;
            float dx = Mathf.Abs(pos.x - center.x);
            float dy = Mathf.Abs(pos.y - center.y);

            if (dx > halfX)
            {
                halfX = dx;
            }

            if (dy > halfY)
            {
                halfY = dy;
            }
        }

        halfX = Mathf.Max(halfX, 0.001f);
        halfY = Mathf.Max(halfY, 0.001f);
    }

    private float GetEdgeScore(Vector2 tilePos, Vector2 center, float halfX, float halfY)
    {
        float nx = Mathf.Abs(tilePos.x - center.x) / halfX;
        float ny = Mathf.Abs(tilePos.y - center.y) / halfY;
        return Mathf.Max(nx, ny);
    }

    private void SetupRandomSeed()
    {
        if (useRandomSeed)
        {
            Random.InitState(System.Environment.TickCount);
            return;
        }

        Random.InitState(fixedSeed);
    }

    private void ApplyDifficulty(int roundNumber)
    {
        int safeRound = Mathf.Max(1, roundNumber);

        float waveScale = Mathf.Pow(waveIntervalMultiplierPerRound, safeRound - 1);
        float crackScale = Mathf.Pow(crackDelayMultiplierPerRound, safeRound - 1);

        float currentInterval = Mathf.Max(minWaveInterval, baseWaveInterval * waveScale);
        waveInterval = currentInterval;

        float currentCrackScale = Mathf.Max(minCrackDelayMultiplier, crackScale);
        currentCrackDelayScale = currentCrackScale;
        ApplyCrackDelayScaleToTiles(currentCrackScale);
    }

    private void ApplyCrackDelayScaleToTiles(float scale)
    {
        if (tiles == null)
        {
            return;
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                tiles[i].SetCrackDelayScale(scale);
            }
        }
    }

    private float GetCurrentWaveInterval()
    {
        return Mathf.Max(minWaveInterval, waveInterval);
    }
}
