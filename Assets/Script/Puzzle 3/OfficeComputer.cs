using UnityEngine;
using System.Collections;

public class OfficeComputer : MonoBehaviour, IInteractable
{
    [SerializeField] private ComputerPopup popup;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer computerRenderer;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;

    [Header("Hack Loading")]
    [SerializeField] private SpriteRenderer loadingRenderer;
    [SerializeField] private Sprite[] loadingSprites;
    [SerializeField] private float timePerFrame = 0.2f;

    private bool hasPower = false;
    private bool isHacking = false;
    private bool hackFinished = false;

    private Puzzle3Manager manager;

    private void Awake()
    {
        manager = FindFirstObjectByType<Puzzle3Manager>();
        UpdateComputerVisual();

        if (loadingRenderer != null)
            loadingRenderer.gameObject.SetActive(false);
    }

    public void SetPower(bool value)
    {
        hasPower = value;
        UpdateComputerVisual();
    }

    private void UpdateComputerVisual()
    {
        if (computerRenderer == null) return;

        computerRenderer.sprite = hasPower ? activeSprite : inactiveSprite;
    }

    public void Interact(GameObject interactor)
    {
        if (!hasPower) return;

        if (isHacking) return;

        // ⭐ kalau sudah hack selesai → buka popup
        if (hackFinished)
        {
            popup.Show(manager.GetUsername(), manager.GetPassword());
            return;
        }

        if (!manager.PlayerHasCard()) return;

        manager.ConsumeCard();

        StartCoroutine(HackRoutine());
    }

    private IEnumerator HackRoutine()
    {
        Debug.Log("HACK START");
        isHacking = true;

        loadingRenderer.gameObject.SetActive(true);

        for (int i = 0; i < loadingSprites.Length; i++)
        {
            loadingRenderer.sprite = loadingSprites[i];
            yield return new WaitForSeconds(timePerFrame);
        }

        loadingRenderer.gameObject.SetActive(false);

        isHacking = false;

        manager.GenerateCredential();
        
        hackFinished = true;

        Debug.Log("Hack Finished");
    }
}