using UnityEngine;

public class pickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    [SerializeField] private string itemId = "AccessCard";
    [SerializeField] private bool usedInPuzzle1 = true;

    public string ItemId => itemId;

    private Puzzle1Manager puzzle1Manager;

    private void Awake()
    {
        puzzle1Manager = FindFirstObjectByType<Puzzle1Manager>();
    }

    public void Interact(GameObject interactor)
    {
        playerCarryItem carryItem = interactor.GetComponent<playerCarryItem>();
        if (carryItem == null)
        {
            Debug.LogWarning("playerCarryItem tidak ditemukan di player.");
            return;
        }

        if (usedInPuzzle1 && puzzle1Manager != null && !puzzle1Manager.CanPickItem(itemId))
        {
            return;
        }

        if (!carryItem.TryPickItem(itemId))
        {
            Debug.Log("Tangan player penuh. Simpan satu item saja.");
            return;
        }

        gameObject.SetActive(false);
    }

    public void ConfigureRuntimeItem(string runtimeItemId, bool forPuzzle1)
    {
        itemId = runtimeItemId;
        usedInPuzzle1 = forPuzzle1;
    }
}
