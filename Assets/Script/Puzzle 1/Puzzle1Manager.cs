using System.Collections.Generic;
using UnityEngine;

public class Puzzle1Manager : MonoBehaviour, IPuzzleRound
{
    [Header("Puzzle 1 Setup")]
    [SerializeField] private bool mustFollowOrder = false;
<<<<<<< Updated upstream
    [SerializeField] private string[] requiredItems = { "mug", "stapler", "book" };
    [SerializeField] private string[] allItemsInGame = { "mug", "stapler", "book", "pen", "notebook", "scissors", "paperclip" };
    [SerializeField, Min(1)] private int requiredItemCountPerRound = 3;
    [SerializeField] private bool randomizeRequiredItemsEachRound = true;
    [Header("Item Source")]
    [SerializeField] private bool usePrefabItems = true;
    [SerializeField] private GameObject[] itemObjectsToReset;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private bool preferRoomBuilderRange = true;
    [SerializeField] private bool uniqueRoomBuilderPositions = true;
    [SerializeField] private Transform[] prefabSpawnPoints;
    [SerializeField] private Transform spawnedItemRoot;
    [SerializeField] private roomBuilder roomGenerator;
=======
    [SerializeField] private string[] requiredItems;
    [SerializeField] private GameObject[] itemObjectsToReset;
    [SerializeField] private string[] allItemsInGame = { "mug", "stapler", "book", "pen", "notebook", "scissors", "paperClip" };
    [SerializeField, Min(1)] private int requiredItemCountPerRound = 3;
    [SerializeField] private bool randomizeRequiredItemsEachRound = true;
>>>>>>> Stashed changes

    private game gameManager;
    private readonly HashSet<string> submittedItems = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
    private readonly List<GameObject> spawnedRuntimeItems = new List<GameObject>();
    private int orderedProgress;

    public bool IsCompleted { get; private set; }
    public int RequiredItemCount => requiredItems != null ? requiredItems.Length : 0;

    private void Awake()
    {
        gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();

        if (requiredItems == null || requiredItems.Length == 0)
        {
            Debug.LogWarning("Puzzle1Manager belum punya daftar item required.");
        }

        if (roomGenerator == null)
        {
            roomGenerator = FindFirstObjectByType<roomBuilder>();
        }
    }

    public void BeginPuzzleRound()
    {
        ResetPuzzleRound();
    }

    public void ResetPuzzleRound()
    {
        IsCompleted = false;
        orderedProgress = 0;
        submittedItems.Clear();

        RandomizeRequiredItemsIfNeeded();

<<<<<<< Updated upstream
        if (usePrefabItems)
=======
        if (itemObjectsToReset == null)
>>>>>>> Stashed changes
        {
            RespawnItemsFromPrefabs();
        }
        else
        {
            ResetSceneItems();
        }

    }

    public bool CanPickItem(string itemId)
    {
        itemId = NormalizeItemId(itemId);

        if (IsCompleted)
        {
            return true;
        }

        if (!IsItemInList(itemId))
        {
            FailPuzzle("Salah ambil item: " + itemId);
            return false;
        }

        if (submittedItems.Contains(itemId))
        {
            FailPuzzle("Item sudah pernah disubmit: " + itemId);
            return false;
        }

        return true;
    }

    public bool TrySubmitItem(string itemId)
    {
        itemId = NormalizeItemId(itemId);

        if (IsCompleted)
        {
            return false;
        }

        if (mustFollowOrder)
        {
            if (requiredItems == null || orderedProgress < 0 || orderedProgress >= requiredItems.Length)
            {
                FailPuzzle("Data urutan item tidak valid.");
                return false;
            }

            string expectedItem = NormalizeItemId(requiredItems[orderedProgress]);
            if (itemId != expectedItem)
            {
                FailPuzzle("Urutan item salah. Seharusnya: " + expectedItem + ", tapi dapat: " + itemId);
                return false;
            }

            submittedItems.Add(itemId);
            orderedProgress++;
        }
        else
        {
            if (!IsItemInList(itemId) || submittedItems.Contains(itemId))
            {
                FailPuzzle("Item submit tidak valid: " + itemId);
                return false;
            }

            submittedItems.Add(itemId);
        }

        Debug.Log("Progress Puzzle 1: " + submittedItems.Count + "/" + requiredItems.Length);

        if (submittedItems.Count >= requiredItems.Length)
        {
            IsCompleted = true;
            Debug.Log("Puzzle 1 selesai. Exit sekarang terbuka.");
        }

        return true;
    }

    private bool IsItemInList(string itemId)
    {
        itemId = NormalizeItemId(itemId);

        if (requiredItems == null)
        {
            return false;
        }

        for (int i = 0; i < requiredItems.Length; i++)
        {
            if (NormalizeItemId(requiredItems[i]) == itemId)
            {
                return true;
            }
        }

        return false;
    }

    private void FailPuzzle(string reason)
    {
        Debug.Log("Puzzle 1 gagal: " + reason);

        if (gameManager != null)
        {
            gameManager.LoseRun();
        }
    }

    private string NormalizeItemId(string itemId)
    {
        return string.IsNullOrWhiteSpace(itemId) ? string.Empty : itemId.Trim().ToLowerInvariant();
    }

    public string[] GetRequiredItemsSnapshot()
    {
        if (requiredItems == null)
        {
            return System.Array.Empty<string>();
        }

        string[] copy = new string[requiredItems.Length];
        for (int i = 0; i < requiredItems.Length; i++)
        {
            copy[i] = requiredItems[i];
        }

        return copy;
    }

<<<<<<< Updated upstream
    public string GetObjectiveTitle()
    {
        return "Objective - Puzzle 1";
    }

    public string GetObjectiveDescription()
    {
        return BuildObjectiveText(true, true);
    }

    public string GetObjectiveTextForPaper()
    {
        return BuildObjectiveText(false, false);
    }

    private string BuildObjectiveText(bool includeProgress, bool includeHint)
    {
        string[] items = GetRequiredItemsSnapshot();
        if (items.Length == 0)
        {
            return "- Kumpulkan item objective";
        }

        string text = "Kumpulkan dan submit item berikut:\n";
        for (int i = 0; i < items.Length; i++)
        {
            string normalized = NormalizeItemId(items[i]);
            bool submitted = submittedItems.Contains(normalized);
            string mark = submitted ? "[x] " : "[ ] ";
            text += "- " + mark + items[i] + "\n";
        }

        if (includeProgress)
        {
            text += "\nProgress: " + submittedItems.Count + "/" + items.Length;
        }

        if (includeHint)
        {
            text += "\nTekan T untuk buka/tutup objective";
        }

        return text.TrimEnd();
    }

    private void RandomizeRequiredItemsIfNeeded()
    {
        if (!randomizeRequiredItemsEachRound)
=======
    private void RandomizeRequiredItemsIfNeeded()
    {
        if (!randomizeRequiredItemsEachRound && requiredItems != null && requiredItems.Length > 0)
>>>>>>> Stashed changes
        {
            return;
        }

        List<string> pool = new List<string>();

        if (allItemsInGame != null)
        {
            for (int i = 0; i < allItemsInGame.Length; i++)
            {
<<<<<<< Updated upstream
                string item = allItemsInGame[i];
                string normalized = NormalizeItemId(item);
=======
                string original = allItemsInGame[i];
                string normalized = NormalizeItemId(original);

>>>>>>> Stashed changes
                if (string.IsNullOrEmpty(normalized))
                {
                    continue;
                }

                bool exists = false;
                for (int j = 0; j < pool.Count; j++)
                {
                    if (NormalizeItemId(pool[j]) == normalized)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
<<<<<<< Updated upstream
                    pool.Add(item.Trim());
=======
                    pool.Add(original.Trim());
>>>>>>> Stashed changes
                }
            }
        }

        if (pool.Count == 0)
        {
<<<<<<< Updated upstream
            Debug.LogWarning("Puzzle1Manager: allItemsInGame kosong. Pakai requiredItems lama.");
            return;
        }

        for (int i = pool.Count - 1; i > 0; i--)
        {
            int swap = Random.Range(0, i + 1);
            string temp = pool[i];
            pool[i] = pool[swap];
            pool[swap] = temp;
        }

        int count = Mathf.Clamp(requiredItemCountPerRound, 1, pool.Count);
        requiredItems = new string[count];

=======
            requiredItems = System.Array.Empty<string>();
            Debug.LogWarning("Puzzle1Manager: allItemsInGame kosong, requiredItems tidak bisa diacak.");
            return;
        }

        int count = Mathf.Clamp(requiredItemCountPerRound, 1, pool.Count);

        for (int i = pool.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            string temp = pool[i];
            pool[i] = pool[swapIndex];
            pool[swapIndex] = temp;
        }

        requiredItems = new string[count];
>>>>>>> Stashed changes
        for (int i = 0; i < count; i++)
        {
            requiredItems[i] = pool[i];
        }

<<<<<<< Updated upstream
        Debug.Log("Puzzle1Manager: Objective round ini -> " + string.Join(", ", requiredItems));
    }

    private void ResetSceneItems()
    {
        if (itemObjectsToReset == null)
        {
            return;
        }

        for (int i = 0; i < itemObjectsToReset.Length; i++)
        {
            if (itemObjectsToReset[i] != null)
            {
                itemObjectsToReset[i].SetActive(true);
            }
        }
    }

    private void RespawnItemsFromPrefabs()
    {
        ClearRuntimeSpawnedItems();

        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("Puzzle1Manager: usePrefabItems aktif tapi itemPrefabs kosong.");
            return;
        }

        string[] targets = GetRequiredItemsSnapshot();
        if (targets.Length == 0)
        {
            return;
        }

        List<int> spawnIndices = new List<int>();
        if (prefabSpawnPoints != null)
        {
            for (int i = 0; i < prefabSpawnPoints.Length; i++)
            {
                if (prefabSpawnPoints[i] != null)
                {
                    spawnIndices.Add(i);
                }
            }
        }

        for (int i = 0; i < targets.Length; i++)
        {
            string targetItemId = targets[i];
            GameObject prefab = FindPrefabForItemId(targetItemId);
            if (prefab == null)
            {
                Debug.LogWarning("Puzzle1Manager: prefab untuk item objective tidak ditemukan: " + targetItemId);
                continue;
            }

            Vector3 spawnPos = transform.position;
            Quaternion spawnRot = Quaternion.identity;

            if (preferRoomBuilderRange && TryGetRoomBuilderSpawnPosition(uniqueRoomBuilderPositions, out Vector3 roomPos))
            {
                spawnPos = roomPos;
            }
            else
            {
                if (spawnIndices.Count > 0)
                {
                    int pick = Random.Range(0, spawnIndices.Count);
                    int idx = spawnIndices[pick];
                    spawnIndices.RemoveAt(pick);

                    Transform spawn = prefabSpawnPoints[idx];
                    if (spawn != null)
                    {
                        spawnPos = spawn.position;
                        spawnRot = spawn.rotation;
                    }
                }
            }

            Transform parent = spawnedItemRoot != null ? spawnedItemRoot : null;
            GameObject instance = Instantiate(prefab, spawnPos, spawnRot, parent);
            ConfigureSpawnedItem(instance, targetItemId);
            spawnedRuntimeItems.Add(instance);
        }
    }

    private void ConfigureSpawnedItem(GameObject instance, string targetItemId)
    {
        if (instance == null)
        {
            return;
        }

        pickupItem pickup = instance.GetComponent<pickupItem>();
        if (pickup == null)
        {
            pickup = instance.GetComponentInChildren<pickupItem>(true);
        }

        if (pickup == null)
        {
            pickup = instance.AddComponent<pickupItem>();
        }

        pickup.ConfigureRuntimeItem(targetItemId, true);
        EnsureItemHasCollider(pickup.gameObject);

        // Keep prefab's original layer setup to avoid breaking player interaction masks.
    }

    private void EnsureItemHasCollider(GameObject instance)
    {
        if (instance == null)
        {
            return;
        }

        Collider2D col = instance.GetComponent<Collider2D>();
        if (col == null)
        {
            BoxCollider2D box = instance.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            return;
        }

        col.isTrigger = true;
    }

    private GameObject FindPrefabForItemId(string targetItemId)
    {
        string target = NormalizeItemId(targetItemId);

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            GameObject prefab = itemPrefabs[i];
            if (prefab == null)
            {
                continue;
            }

            pickupItem pickup = prefab.GetComponent<pickupItem>();
            if (pickup == null)
            {
                pickup = prefab.GetComponentInChildren<pickupItem>(true);
            }

            if (pickup == null)
            {
                continue;
            }

            if (NormalizeItemId(pickup.ItemId) == target)
            {
                return prefab;
            }
        }

        return null;
    }

    private void ClearRuntimeSpawnedItems()
    {
        for (int i = 0; i < spawnedRuntimeItems.Count; i++)
        {
            if (spawnedRuntimeItems[i] != null)
            {
                Destroy(spawnedRuntimeItems[i]);
            }
        }

        spawnedRuntimeItems.Clear();
    }

    private bool TryGetRoomBuilderSpawnPosition(bool reservePosition, out Vector3 position)
    {
        position = default;

        if (roomGenerator == null)
        {
            roomGenerator = FindFirstObjectByType<roomBuilder>();
        }

        if (roomGenerator == null)
        {
            return false;
        }

        return roomGenerator.TryGetGeneratedSpawnPosition(reservePosition, out position);
=======
        Debug.Log("Puzzle1Manager: requiredItems round ini -> " + string.Join(", ", requiredItems));
    }

    public string GetObjectiveTextForPaper()
    {
        return BuildObjectiveText("OBJECTIVE SUBMIT:", false, false);
    }

    public string GetObjectiveTextForPopup()
    {
        return BuildObjectiveText("Daftar item yang harus disubmit:", true, true);
    }

    private string BuildObjectiveText(string title, bool includeProgress, bool includeCloseHint)
    {
        string[] items = GetRequiredItemsSnapshot();

        if (items.Length == 0)
        {
            return title + "\n- Belum ada item";
        }

        string text = title + "\n";

        for (int i = 0; i < items.Length; i++)
        {
            string normalized = NormalizeItemId(items[i]);
            bool submitted = submittedItems.Contains(normalized);
            string mark = includeProgress ? (submitted ? "[x] " : "[ ] ") : string.Empty;
            text += "- " + mark + items[i] + "\n";
        }

        if (includeProgress)
        {
            text += "\nProgress: " + submittedItems.Count + "/" + items.Length;
        }

        if (includeCloseHint)
        {
            text += "\nTekan ESC untuk menutup";
        }

        return text.TrimEnd();
>>>>>>> Stashed changes
    }
}
