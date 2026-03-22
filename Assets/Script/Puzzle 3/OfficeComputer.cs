using UnityEngine;

public class OfficeComputer : MonoBehaviour, IInteractable
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;

    private bool hasPower = false;
    private Puzzle3Manager manager;

    private void Awake()
    {
        manager = FindFirstObjectByType<Puzzle3Manager>();
        UpdateVisual();
    }

    public void SetPower(bool value)
    {
        hasPower = value;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = hasPower ? activeSprite : inactiveSprite;
    }

    public void Interact(GameObject interactor)
    {
        // ❌ kalau belum nyala → tidak bisa apa-apa
        if (!hasPower) return;

        // ❌ kalau player tidak punya kartu
        if (!manager.PlayerHasCard()) return;

        // pakai kartu
        manager.ConsumeCard();

        StartComputerUse();
    }

    private void StartComputerUse()
    {
        Debug.Log("Computer accessed → start hacking later");
    }
}