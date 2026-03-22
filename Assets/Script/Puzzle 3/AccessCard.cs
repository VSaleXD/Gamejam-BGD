using UnityEngine;

public class AccessCard : MonoBehaviour, IInteractable
{
    private bool taken = false;
    private Puzzle3Manager manager;

    private void Start()
    {
        manager = FindFirstObjectByType<Puzzle3Manager>();
    }

    public void Interact(GameObject interactor)
    {
        if (taken) return;

        taken = true;

        manager.OnCardTaken(interactor);

        gameObject.SetActive(false);
    }

    public void ResetCardState()
    {
        taken = false;
        gameObject.SetActive(true);
    }
}