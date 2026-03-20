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

    [Header("Pressure Pattern")]
    [SerializeField] private PressureMode pressureMode = PressureMode.EdgeToCenter;
    [SerializeField, Range(0f, 0.5f)] private float edgeBandWidth = 0.08f;
    [SerializeField] private Transform centerPoint;

    [Header("Tile Source")]
    [SerializeField] private bool autoFindTilesFromChildren = true;
    [SerializeField] private Transform tilesRoot;
    [SerializeField] private floorRetak[] tiles;

    [Header("Random")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int fixedSeed = 12345;

    private float timer;
    private bool started;

    private void Start()
    {
        SetupRandomSeed();
        RefreshTiles();

        timer = initialDelay;
        started = runOnStart;
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
        timer = waveInterval;
    }

    public void StartPressure()
    {
        started = true;
        timer = initialDelay;
    }

    public void StopPressure()
    {
        started = false;
    }

    public void RefreshTiles()
    {
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

    public void TriggerPressureWave()
    {
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
}
