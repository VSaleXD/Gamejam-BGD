using System.Collections.Generic;
using UnityEngine;

public class Puzzle1Manager : MonoBehaviour, IPuzzleRound
{
    [Header("Puzzle 1 Setup")]
    [SerializeField] private bool mustFollowOrder = false;
    [SerializeField] private string[] requiredItems = { "mug", "stapler", "book" };
    [SerializeField] private GameObject[] itemObjectsToReset;

    private game gameManager;
    private readonly HashSet<string> submittedItems = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
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
}
