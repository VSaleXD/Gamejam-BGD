using UnityEngine;
using TMPro;

public class ComputerPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI passwordText;

    public void Show(string user, string pass)
    {
        gameObject.SetActive(true);

        usernameText.text = "USER : " + user;
        passwordText.text = "PASS : " + pass;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // ⭐ dipanggil tombol X
    public void ClosePopup()
    {
        Hide();
    }
}