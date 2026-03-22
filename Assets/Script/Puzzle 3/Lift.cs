using UnityEngine;

public class Lift : MonoBehaviour, IInteractable
{
    [SerializeField] private LiftLoginPopup popup;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    private Puzzle3Manager manager;

    private bool unlocked = false;

    private void Awake()
    {
        manager = FindFirstObjectByType<Puzzle3Manager>();
    }

    public void Interact(GameObject interactor)
    {
        if (unlocked)
        {
            GoToNextFloor();
            return;
        }

        popup.SetLift(this);
        popup.Show();
    }

    public void AttemptLogin(string user, string pass)
    {
        string correctUser = manager.GetUsername();
        string correctPass = manager.GetPassword();

        bool userCorrect = user == correctUser;
        bool passCorrect = pass == correctPass;

        if (userCorrect && passCorrect)
        {
            unlocked = true;

            spriteRenderer.sprite = openSprite;

            popup.Hide();

            Debug.Log("Lift Unlocked!");
            return;
        }

        if (!userCorrect && !passCorrect)
        {
            popup.ShowError("Incorrect username and password!");
        }
        else if (!userCorrect)
        {
            popup.ShowError("Incorrect username!");
        }
        else if (!passCorrect)
        {
            popup.ShowError("Incorrect password!");
        }
    }

    private void EnterLift()
    {
        Debug.Log("Go to next room");
    }

    private void GoToNextFloor()
    {
        Debug.Log("Enter Lift");

        if (game.Instance != null)
        {
            Debug.Log("CALL WINRUN");
            game.Instance.WinRun();
        }
        else
        {
            Debug.LogWarning("Game Manager tidak ditemukan!");
        }
    }
}