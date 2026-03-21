using UnityEngine;

public class exit : MonoBehaviour, IInteractable
{
    [Header("Objective Gate")]
    [SerializeField] private bool requiresCurrentPuzzleComplete = true;

    [Header("Exit Requirement")]
    [SerializeField] private bool requiresItem = false;
    [SerializeField] private string requiredItemId = "AccessCard";

    private game gameManager;

    private void Awake()
    {
        gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();
    }

    public void Interact(GameObject interactor)
    {
        playerCarryItem carryItem = interactor.GetComponent<playerCarryItem>();

        if (requiresCurrentPuzzleComplete && gameManager != null && !gameManager.IsCurrentPuzzleCompleted())
        {
            Debug.Log("Exit terkunci. Selesaikan objective puzzle aktif dulu.");
            return;
        }

        if (requiresItem)
        {
            if (carryItem == null || !carryItem.HasRequiredItem(requiredItemId))
            {
                Debug.Log("Exit terkunci. Butuh item: " + requiredItemId);
                return;
            }

            carryItem.ClearItem();
        }

        Debug.Log("Exit terbuka. Player berhasil keluar ruangan.");

        if (gameManager != null)
        {
            gameManager.WinRun();
        }
    }
}
