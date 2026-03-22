using UnityEngine;
using UnityEngine.UI;

public class Puzzle1ObjectivePopupUI : MonoBehaviour
{
    [Header("Popup References")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;

    [Header("Input")]
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;

    public bool IsOpen => popupRoot != null && popupRoot.activeSelf;

    private void Awake()
    {
        if (popupRoot != null)
        {
            popupRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOpen)
        {
            return;
        }

        if (Input.GetKeyDown(closeKey))
        {
            Hide();
        }
    }

    public void Toggle(Puzzle1Manager puzzle1Manager)
    {
        if (IsOpen)
        {
            Hide();
            return;
        }

        Show(puzzle1Manager);
    }

    public void Show(Puzzle1Manager puzzle1Manager)
    {
        if (popupRoot == null)
        {
            Debug.LogWarning("Puzzle1ObjectivePopupUI: popupRoot belum di-assign.");
            return;
        }

        if (titleText != null)
        {
            titleText.text = "Objective Puzzle 1";
        }

        if (bodyText != null)
        {
            bodyText.text = puzzle1Manager != null
                ? puzzle1Manager.GetObjectiveTextForPopup()
                : "- Kumpulkan item objective";
        }

        popupRoot.SetActive(true);
    }

    public void Hide()
    {
        if (popupRoot != null)
        {
            popupRoot.SetActive(false);
        }
    }
}
