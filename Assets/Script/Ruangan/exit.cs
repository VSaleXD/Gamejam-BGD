using UnityEngine;
using TMPro;

public class exit : MonoBehaviour, IInteractable
{
    [Header("Objective Gate")]
    [SerializeField] private bool requiresCurrentPuzzleComplete = true;

    [Header("Exit Requirement")]
    [SerializeField] private bool requiresItem = false;
    [SerializeField] private string requiredItemId = "AccessCard";

    [Header("Lift Login (Puzzle 3)")]
    [SerializeField] private bool requiresLiftLogin = false;
    [SerializeField] private MonoBehaviour liftLoginPopupBehaviour;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    private game gameManager;
    private Puzzle3Manager puzzle3Manager;
    private bool liftUnlocked;

    private void Awake()
    {
        gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();
        puzzle3Manager = FindFirstObjectByType<Puzzle3Manager>();

        ResolveLiftLoginPopupIfMissing();
    }

    public void Interact(GameObject interactor)
    {
        if (requiresLiftLogin && !liftUnlocked)
        {
            ShowLiftLogin();
            return;
        }

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

    public void AttemptLiftLogin(string user, string pass)
    {
        if (!requiresLiftLogin)
        {
            liftUnlocked = true;
            HideLiftLogin();
            return;
        }

        if (puzzle3Manager == null)
        {
            puzzle3Manager = FindFirstObjectByType<Puzzle3Manager>();
        }

        if (puzzle3Manager == null)
        {
            Debug.LogWarning("Exit: Puzzle3Manager tidak ditemukan, login lift belum bisa divalidasi.");
            return;
        }

        string expectedUser = puzzle3Manager.GetUsername();
        string expectedPass = puzzle3Manager.GetPassword();

        if (string.IsNullOrEmpty(expectedUser) || string.IsNullOrEmpty(expectedPass))
        {
            Debug.Log("Credential belum dibuat. Selesaikan komputer dulu.");
            return;
        }

        if (user == expectedUser && pass == expectedPass)
        {
            liftUnlocked = true;
            HideLiftLogin();
            Debug.Log("Lift Unlocked!");
            return;
        }

        Debug.Log("Wrong Credential");
    }

    public void TryLoginFromPopup()
    {
        string user = usernameInput != null ? usernameInput.text : string.Empty;
        string pass = passwordInput != null ? passwordInput.text : string.Empty;
        AttemptLiftLogin(user, pass);
    }

    public void ShowLiftLogin()
    {
        ResolveLiftLoginPopupIfMissing();

        if (liftLoginPopupBehaviour != null)
        {
            liftLoginPopupBehaviour.gameObject.SetActive(true);
        }

        if (usernameInput != null)
        {
            usernameInput.text = string.Empty;
        }

        if (passwordInput != null)
        {
            passwordInput.text = string.Empty;
        }
    }

    public void HideLiftLogin()
    {
        if (liftLoginPopupBehaviour != null)
        {
            liftLoginPopupBehaviour.gameObject.SetActive(false);
        }
    }

    private void ResolveLiftLoginPopupIfMissing()
    {
        if (liftLoginPopupBehaviour != null)
        {
            return;
        }

        MonoBehaviour[] allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < allBehaviours.Length; i++)
        {
            MonoBehaviour behaviour = allBehaviours[i];
            if (behaviour != null && behaviour.GetType().Name == "LiftLoginPopup")
            {
                liftLoginPopupBehaviour = behaviour;
                break;
            }
        }
    }
}
