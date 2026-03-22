using UnityEngine;
using UnityEngine.Events;   // ⭐ TAMBAHAN
using System.Collections.Generic;

public class Puzzle2Manager : MonoBehaviour, IPuzzleRound
{
    [Header("Puzzle 2 Setup")]
    [SerializeField] private PowerSocket[] sockets;

    [Header("Socket Random Spawn")]
    [SerializeField] private bool randomizeSocketSpawn = true;
    [SerializeField] private bool uniqueSpawnPerSocket = true;
    [SerializeField] private Transform[] socketSpawnPoints;

    [Header("GLOBAL ACTIVATION (isi di inspector)")]
    [SerializeField] private UnityEvent onPuzzleCompleted;   // ⭐ TAMBAHAN

    [Header("Lift Unlock")]
    [SerializeField] private bool autoUnlockLiftOnCompleted = true;
    [SerializeField] private exit exitToUnlock;

    public bool IsCompleted { get; private set; }

    public static Puzzle2Manager Instance;

    private void Awake()
    {
        Instance = this;

        if (sockets == null || sockets.Length == 0)
        {
            sockets = GetComponentsInChildren<PowerSocket>(true);
        }

        if (exitToUnlock == null)
        {
            exitToUnlock = FindFirstObjectByType<exit>(FindObjectsInactive.Include);
        }
    }

    private void Update()
    {
        if (IsCompleted) return;

        if (AreAllSocketsPowered())
        {
            IsCompleted = true;

            Debug.Log("Puzzle 2 selesai. Semua socket sudah aktif.");

            if (autoUnlockLiftOnCompleted && exitToUnlock != null)
            {
                exitToUnlock.ForceUnlockExit();
            }

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

        RandomizeSocketPositions();

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

    private void RandomizeSocketPositions()
    {
        if (!randomizeSocketSpawn || sockets == null || sockets.Length == 0)
        {
            return;
        }

        List<Transform> validSpawnPoints = new List<Transform>();
        if (socketSpawnPoints != null)
        {
            for (int i = 0; i < socketSpawnPoints.Length; i++)
            {
                if (socketSpawnPoints[i] != null)
                {
                    validSpawnPoints.Add(socketSpawnPoints[i]);
                }
            }
        }

        if (validSpawnPoints.Count == 0)
        {
            RandomizeUsingExistingSocketPositions();
            return;
        }

        if (uniqueSpawnPerSocket)
        {
            Shuffle(validSpawnPoints);

            int count = Mathf.Min(sockets.Length, validSpawnPoints.Count);
            for (int i = 0; i < count; i++)
            {
                if (sockets[i] != null)
                {
                    sockets[i].transform.position = validSpawnPoints[i].position;
                }
            }

            return;
        }

        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null)
            {
                continue;
            }

            int rand = Random.Range(0, validSpawnPoints.Count);
            sockets[i].transform.position = validSpawnPoints[rand].position;
        }
    }

    private void RandomizeUsingExistingSocketPositions()
    {
        List<Vector3> existingPositions = new List<Vector3>();
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] != null)
            {
                existingPositions.Add(sockets[i].transform.position);
            }
        }

        if (existingPositions.Count <= 1)
        {
            return;
        }

        Shuffle(existingPositions);

        int writeIndex = 0;
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null)
            {
                continue;
            }

            sockets[i].transform.position = existingPositions[writeIndex];
            writeIndex++;
        }
    }

    private void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            Transform temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    private void Shuffle(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            Vector3 temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
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