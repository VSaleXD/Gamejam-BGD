using UnityEngine;
using UnityEngine.Events;   // ⭐ TAMBAHAN

public class Puzzle2Manager : MonoBehaviour, IPuzzleRound
{
    [Header("Puzzle 2 Setup")]
    [SerializeField] private PowerSocket[] sockets;

    [Header("GLOBAL ACTIVATION (isi di inspector)")]
    [SerializeField] private UnityEvent onPuzzleCompleted;   // ⭐ TAMBAHAN

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

            // ⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐
            // ⭐ DI SINI GLOBAL EVENT DIPANGGIL
            // ⭐ Contoh:
            // ⭐ - buka lift
            // ⭐ - lanjut ke room berikutnya
            // ⭐ - nyalakan generator utama
            // ⭐ - trigger cutscene
            // ⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐⭐
            onPuzzleCompleted?.Invoke();
        }
    }

    public void BeginPuzzleRound()
    {
        ResetPuzzleRound();
    }

    public void ResetPuzzleRound()
    {
        IsCompleted = false;

        // ⭐ reset socket
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] != null)
                sockets[i].ResetPower();
        }

        // ⭐ destroy semua kabel
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

    public string GetObjectiveTitle()
    {
        return "Objective - Puzzle 2";
    }

    public string GetObjectiveDescription()
    {
        int total = sockets != null ? sockets.Length : 0;
        int powered = 0;

        if (sockets != null)
        {
            for (int i = 0; i < sockets.Length; i++)
            {
                if (sockets[i] != null && sockets[i].isPowered)
                {
                    powered++;
                }
            }
        }

        string text = "Sambungkan listrik sampai semua socket aktif.\n";
        text += "Progress Socket: " + powered + "/" + total + "\n";
        text += "Tekan T untuk buka/tutup objective";
        return text;
    }
}