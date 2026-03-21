using UnityEngine;

public class PowerSource : MonoBehaviour, IInteractable
{
    [Header("Cable Mode")]
    [SerializeField] private bool useColorCycle = true;
    [SerializeField] private Color fixedColor = Color.yellow;

    private int colorIndex = 0;

    private Color[] colors =
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow
    };

    private bool fixedCableAlreadyUsed = false;

    public void Interact(GameObject interactor)
    {
        if (Puzzle2Manager.Instance != null &&
            Puzzle2Manager.Instance.IsCompleted)
            return;

        // ⭐ kalau mode fixed dan sudah pernah dipakai → stop
        if (!useColorCycle && fixedCableAlreadyUsed)
            return;

        PlayerCable pc = interactor.GetComponentInChildren<PlayerCable>();
        if (pc == null) return;

        if (useColorCycle)
        {
            pc.BeginCable(transform, colors[colorIndex]);
            CycleColor();
        }
        else
        {
            pc.BeginCable(transform, fixedColor);
        }
    }

    void CycleColor()
    {
        colorIndex++;
        if (colorIndex >= colors.Length)
            colorIndex = 0;
    }

    // ⭐ dipanggil saat kabel berhasil dipasang
    public void NotifyCableUsed()
    {
        fixedCableAlreadyUsed = true;
    }
}