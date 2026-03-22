using UnityEngine;

public class Puzzle1ObjectivePaperInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Puzzle1Manager puzzle1Manager;
    [SerializeField] private Puzzle1ObjectivePopupUI popupUI;

    private void Awake()
    {
        if (puzzle1Manager == null)
        {
            puzzle1Manager = FindFirstObjectByType<Puzzle1Manager>();
        }

        if (popupUI == null)
        {
            popupUI = FindFirstObjectByType<Puzzle1ObjectivePopupUI>();
        }
    }

    public void Interact(GameObject interactor)
    {
        if (popupUI == null)
        {
            Debug.LogWarning("Puzzle1ObjectivePaperInteract: Popup UI belum ditemukan.");
            return;
        }

        popupUI.Toggle(puzzle1Manager);
    }

    public void Configure(Puzzle1Manager manager, Puzzle1ObjectivePopupUI ui)
    {
        puzzle1Manager = manager;
        popupUI = ui;
    }
}
