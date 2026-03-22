using UnityEngine;

public class Lift : MonoBehaviour, IInteractable
{
    [SerializeField] private LiftLoginPopup popup;

    private Puzzle3Manager manager;

    private bool unlocked = false;

    private void Awake()
    {
        manager = FindFirstObjectByType<Puzzle3Manager>();
    }

    public void Interact(GameObject interactor)
    {
        // ⭐ kalau sudah unlock → masuk lift
        if (unlocked)
        {
            EnterLift();
            return;
        }

        // ⭐ kalau belum unlock → buka login UI
        popup.SetLift(this);
        popup.Show();
    }

    public void AttemptLogin(string user, string pass)
    {
        if (user == manager.GetUsername() &&
            pass == manager.GetPassword())
        {
            unlocked = true;

            popup.Hide();

            Debug.Log("Lift Unlocked!");
        }
        else
        {
            Debug.Log("Wrong Credential");
        }
    }

    private void EnterLift()
    {
        Debug.Log("Go to next room");
    }
}