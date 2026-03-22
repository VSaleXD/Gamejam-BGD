using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LiftLoginPopup : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private exit exitGate;

    public void SetExit(exit targetExit)
    {
        exitGate = targetExit;
    }

    private void Awake()
    {
        if (exitGate == null)
        {
            exitGate = GetComponentInParent<exit>();
        }

        if (exitGate == null)
        {
            exitGate = FindFirstObjectByType<exit>();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        usernameInput.text = "";
        passwordInput.text = "";

        errorText.text = "";

        usernameInput.ActivateInputField();

        GameInputLock.InputLocked = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        GameInputLock.InputLocked = false;
    }

    private void Update()
{
    if (!gameObject.activeSelf) return;

    if (Keyboard.current != null &&
        Keyboard.current.enterKey.wasPressedThisFrame)
    {
        TryLogin();
    }
}

    public void TryLogin()
    {
        if (exitGate == null)
        {
            ShowError("Exit gate belum terhubung.");
            return;
        }

        exitGate.AttemptLiftLogin(usernameInput.text, passwordInput.text);
    }

    public void ShowError(string msg)
    {
        errorText.text = msg;
    }

    public void Close()
    {
        Hide();
    }
}