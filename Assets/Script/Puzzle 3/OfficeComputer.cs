using UnityEngine;
using System.Collections;

public class OfficeComputer : MonoBehaviour, IInteractable
{
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

        // ⭐ kalau sudah hack selesai → nanti popup
        if (hackFinished)
        {
            Debug.Log("Show popup later");
            return;
        }

        // ⭐ kalau sedang hacking → ignore
        if (isHacking) return;

        // ⭐ butuh kartu
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
        hackFinished = true;

        Debug.Log("Hack Finished");
    }
}