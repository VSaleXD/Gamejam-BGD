using UnityEngine;

public class Puzzle2Manager : MonoBehaviour, IPuzzleRound
{
    [Header("Puzzle 2 Setup")]
    [SerializeField] private PowerSocket[] sockets;

    public bool IsCompleted { get; private set; }

    public static Puzzle2Manager Instance;

    private void Awake()
    {
        Instance = this;

        if (sockets == null || sockets.Length == 0)
        {
            sockets = GetComponentsInChildren<PowerSocket>(true);
        }
    }

    private void Update()
    {
        if (IsCompleted) return;

        if (AreAllSocketsPowered())
        {
            IsCompleted = true;
            Debug.Log("Puzzle 2 selesai. Semua socket sudah aktif.");
        }
    }

    public void BeginPuzzleRound()
    {
        ResetPuzzleRound();
    }

    public void ResetPuzzleRound()
    {
        IsCompleted = false;

        // reset socket
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] != null)
                sockets[i].ResetPower();
        }

        // destroy semua kabel di dunia
        CableInstance[] cables = FindObjectsByType<CableInstance>(
            FindObjectsSortMode.None
        );

        for (int i = 0; i < cables.Length; i++)
        {
            Destroy(cables[i].gameObject);
        }
    }

    private bool AreAllSocketsPowered()
    {
        if (sockets == null || sockets.Length == 0)
            return false;

        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null || !sockets[i].isPowered)
                return false;
        }

        return true;
    }
}