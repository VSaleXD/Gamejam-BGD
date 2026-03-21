using System.Collections.Generic;
using UnityEngine;

public class RunRandomizer : MonoBehaviour
{
    [Header("Run Randomizer")]
    [SerializeField] private bool randomizeOnStart = true;
    [SerializeField] private bool uniqueSpawnPoints = true;

    [Header("Spawn Safety")]
    [SerializeField] private bool avoidObstacleOverlap = true;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private float spawnCheckRadius = 0.35f;

    [Header("Item Randomization")]
    [SerializeField] private Transform[] itemTargets;
    [SerializeField] private Transform[] itemSpawnPoints;
    [SerializeField] private bool allowGeneratedRoomFallback = true;

    [Header("Generator Fallback")]
    [SerializeField] private roomBuilder roomGenerator;

    [Header("Submit Randomization")]
    [SerializeField] private bool randomizeSubmit = false;
    [SerializeField] private Transform submitTarget;
    [SerializeField] private Transform[] submitSpawnPoints;

    private void Start()
    {
        if (roomGenerator == null)
        {
            roomGenerator = FindFirstObjectByType<roomBuilder>();
        }

        if (randomizeOnStart)
        {
            RandomizeRun();
        }
    }

    public void RandomizeRun()
    {
        RandomizeItems();

        if (randomizeSubmit)
        {
            RandomizeSingleTarget(submitTarget, submitSpawnPoints, "submit");
        }
    }

    private void RandomizeItems()
    {
        if (itemTargets == null || itemTargets.Length == 0)
        {
            Debug.LogWarning("RunRandomizer: itemTargets masih kosong.");
            return;
        }

        if (itemSpawnPoints == null || itemSpawnPoints.Length == 0)
        {
            Debug.LogWarning("RunRandomizer: itemSpawnPoints masih kosong.");
            return;
        }

        if (uniqueSpawnPoints && itemSpawnPoints.Length < itemTargets.Length)
        {
            Debug.LogWarning("RunRandomizer: spawn point item kurang dari jumlah item. Sebagian item akan berbagi posisi.");
        }

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < itemSpawnPoints.Length; i++)
        {
            availableIndices.Add(i);
        }

        for (int i = 0; i < itemTargets.Length; i++)
        {
            Transform target = itemTargets[i];
            if (target == null)
            {
                continue;
            }

            if (!TryPickSpawnIndex(itemSpawnPoints, availableIndices, out int spawnIndex))
            {
                if (TryPlaceWithGeneratedRoomFallback(target, uniqueSpawnPoints))
                {
                    continue;
                }

                Debug.LogWarning("RunRandomizer: tidak menemukan spawn item yang aman dari obstacle untuk target index " + i + ".");
                continue;
            }

            Transform spawnPoint = itemSpawnPoints[spawnIndex];
            if (spawnPoint == null)
            {
                continue;
            }

            target.position = spawnPoint.position;
        }
    }

    private void RandomizeSingleTarget(Transform target, Transform[] spawnPoints, string label)
    {
        if (target == null)
        {
            Debug.LogWarning("RunRandomizer: target " + label + " belum di-assign.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("RunRandomizer: spawn points " + label + " masih kosong.");
            return;
        }

        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            indices.Add(i);
        }

        if (!TryPickSpawnIndex(spawnPoints, indices, out int index))
        {
            Debug.LogWarning("RunRandomizer: tidak menemukan spawn point aman untuk " + label + ".");
            return;
        }

        Transform spawnPoint = spawnPoints[index];

        if (spawnPoint == null)
        {
            Debug.LogWarning("RunRandomizer: spawn point " + label + " null.");
            return;
        }

        target.position = spawnPoint.position;
    }

    private bool TryPickSpawnIndex(Transform[] spawnPoints, List<int> candidateIndices, out int chosenIndex)
    {
        chosenIndex = -1;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return false;
        }

        if (candidateIndices == null || candidateIndices.Count == 0)
        {
            return false;
        }

        List<int> validIndices = new List<int>();

        for (int i = 0; i < candidateIndices.Count; i++)
        {
            int index = candidateIndices[i];
            if (index < 0 || index >= spawnPoints.Length)
            {
                continue;
            }

            Transform spawnPoint = spawnPoints[index];
            if (spawnPoint == null)
            {
                continue;
            }

            if (!IsSpawnBlocked(spawnPoint.position))
            {
                validIndices.Add(index);
            }
        }

        if (validIndices.Count == 0)
        {
            return false;
        }

        chosenIndex = validIndices[Random.Range(0, validIndices.Count)];

        if (uniqueSpawnPoints)
        {
            candidateIndices.Remove(chosenIndex);
        }

        return true;
    }

    private bool IsSpawnBlocked(Vector3 spawnPosition)
    {
        if (!avoidObstacleOverlap)
        {
            return false;
        }

        if (obstacleLayers.value == 0)
        {
            return false;
        }

        Collider2D hit = Physics2D.OverlapCircle(spawnPosition, spawnCheckRadius, obstacleLayers);
        return hit != null;
    }

    private bool TryPlaceWithGeneratedRoomFallback(Transform target, bool reservePosition)
    {
        if (!allowGeneratedRoomFallback)
        {
            return false;
        }

        if (roomGenerator == null)
        {
            roomGenerator = FindFirstObjectByType<roomBuilder>();
        }

        if (roomGenerator == null)
        {
            return false;
        }

        if (!roomGenerator.TryGetGeneratedSpawnPosition(reservePosition, out Vector3 generatedPosition))
        {
            return false;
        }

        target.position = generatedPosition;
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (itemSpawnPoints == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;

        for (int i = 0; i < itemSpawnPoints.Length; i++)
        {
            if (itemSpawnPoints[i] == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(itemSpawnPoints[i].position, spawnCheckRadius);
        }
    }
}
