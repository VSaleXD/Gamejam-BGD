using UnityEngine;
using TMPro;

public class LiftLoginPopup : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    private Lift lift;

    public void SetLift(Lift l)
    {
        lift = l;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        usernameInput.text = "";
        passwordInput.text = "";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void TryLogin()
    {
        if (lift == null) return;

        lift.AttemptLogin(usernameInput.text, passwordInput.text);
    }

    public void Close()
    {
        Hide();
    }
}