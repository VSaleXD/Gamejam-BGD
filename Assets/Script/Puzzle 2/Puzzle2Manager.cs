using UnityEngine;

public class Puzzle2Manager : MonoBehaviour, IPuzzleRound
{
    [Header("Puzzle 2 Setup")]
    [SerializeField] private PowerSocket[] sockets;
    [SerializeField] private PlayerCable[] cables;

    public bool IsCompleted { get; private set; }

    private void Awake()
    {
        if (sockets == null || sockets.Length == 0)
        {
            sockets = GetComponentsInChildren<PowerSocket>(true);
        }

        if (cables == null || cables.Length == 0)
        {
            cables = FindObjectsByType<PlayerCable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
    }

    private void Update()
    {
        if (IsCompleted)
        {
            return;
        }

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

        if (sockets != null)
        {
            for (int i = 0; i < sockets.Length; i++)
            {
                if (sockets[i] != null)
                {
                    sockets[i].ResetPower();
                }
            }
        }

        if (cables != null)
        {
            for (int i = 0; i < cables.Length; i++)
            {
                if (cables[i] != null)
                {
                    cables[i].ResetCable();
                }
            }
        }
    }

    private bool AreAllSocketsPowered()
    {
        if (sockets == null || sockets.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null || !sockets[i].isPowered)
            {
                return false;
            }
        }

        return true;
    }
}
