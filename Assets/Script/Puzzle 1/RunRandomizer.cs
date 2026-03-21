using System.Collections.Generic;
using UnityEngine;

public class RunRandomizer : MonoBehaviour
{
    [Header("Run Randomizer")]
    [SerializeField] private bool randomizeOnStart = true;
    [SerializeField] private bool uniqueSpawnPoints = true;

    [Header("Item Randomization")]
    [SerializeField] private Transform[] itemTargets;
    [SerializeField] private Transform[] itemSpawnPoints;

    [Header("Submit Randomization")]
    [SerializeField] private bool randomizeSubmit = false;
    [SerializeField] private Transform submitTarget;
    [SerializeField] private Transform[] submitSpawnPoints;

    private void Start()
    {
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

            int spawnIndex;

            if (uniqueSpawnPoints && availableIndices.Count > 0)
            {
                int pick = Random.Range(0, availableIndices.Count);
                spawnIndex = availableIndices[pick];
                availableIndices.RemoveAt(pick);
            }
            else
            {
                spawnIndex = Random.Range(0, itemSpawnPoints.Length);
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

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        if (spawnPoint == null)
        {
            Debug.LogWarning("RunRandomizer: spawn point " + label + " null.");
            return;
        }

        target.position = spawnPoint.position;
    }
}
