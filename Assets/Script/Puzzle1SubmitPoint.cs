using UnityEngine;

public class Puzzle1SubmitPoint : MonoBehaviour, IInteractable
{
    private Puzzle1Manager puzzle1Manager;

    private void Awake()
    {
        puzzle1Manager = game.Instance != null ? game.Instance.Puzzle1 : FindFirstObjectByType<Puzzle1Manager>();
    }

    public void Interact(GameObject interactor)
    {
        playerCarryItem carryItem = interactor.GetComponent<playerCarryItem>();
        if (carryItem == null || !carryItem.HasItem)
        {
            Debug.Log("Tidak ada item yang dibawa untuk disubmit.");
            return;
        }

        if (puzzle1Manager == null)
        {
            Debug.LogWarning("Puzzle1Manager tidak ditemukan di scene.");
            return;
        }

        if (puzzle1Manager.TrySubmitItem(carryItem.CarriedItemId))
        {
            carryItem.ClearItem();
        }
    }
}
