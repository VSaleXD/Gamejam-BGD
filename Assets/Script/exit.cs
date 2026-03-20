using UnityEngine;

public class exit : MonoBehaviour, IInteractable
{
    [Header("Objective Gate")]
    [SerializeField] private bool requiresPuzzle1Complete = true;

    [Header("Exit Requirement")]
    [SerializeField] private bool requiresItem = false;
    [SerializeField] private string requiredItemId = "AccessCard";

    private Puzzle1Manager puzzle1Manager;
    private game gameManager;

    private void Awake()
    {
        gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();
        puzzle1Manager = gameManager != null ? gameManager.Puzzle1 : FindFirstObjectByType<Puzzle1Manager>();
    }

    public void Interact(GameObject interactor)
    {
        playerCarryItem carryItem = interactor.GetComponent<playerCarryItem>();

        if (requiresPuzzle1Complete && puzzle1Manager != null && !puzzle1Manager.IsCompleted)
        {
            Debug.Log("Exit terkunci. Selesaikan objective Puzzle 1 dulu.");
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
