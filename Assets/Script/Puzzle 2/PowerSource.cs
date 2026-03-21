using UnityEngine;

public class PowerSource : MonoBehaviour, IInteractable
{
    public enum CableColorType
    {
        Default,
        Red,
        Blue,
        Green,
        Yellow
    }

    [Header("Cable Mode")]
    [SerializeField] private bool useColorCycle = true;
    [SerializeField] private CableColorType fixedColor = CableColorType.Yellow;

    private int colorIndex = 0;

    private CableColorType[] colors =
    {
        CableColorType.Red,
        CableColorType.Blue,
        CableColorType.Green,
        CableColorType.Yellow
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
            pc.BeginCable(transform, ConvertColor(colors[colorIndex]));
            CycleColor();
        }
        else
        {
            pc.BeginCable(transform, ConvertColor(fixedColor));
        }
    }

    void CycleColor()
    {
        colorIndex++;
        if (colorIndex >= colors.Length)
            colorIndex = 0;
    }

    Color ConvertColor(CableColorType t)
    {
        switch (t)
        {
            case CableColorType.Red: return Color.red;
            case CableColorType.Blue: return Color.blue;
            case CableColorType.Green: return Color.green;
            case CableColorType.Yellow: return Color.yellow;
            default: return Color.gray; // ⭐ default
        }
    }

    // ⭐ dipanggil saat kabel berhasil dipasang
    public void NotifyCableUsed()
    {
        fixedCableAlreadyUsed = true;
    }
}