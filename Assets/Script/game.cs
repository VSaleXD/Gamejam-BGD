using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class game : MonoBehaviour
{
    public static game Instance { get; private set; }

    private static bool persistentRunInitialized;
    private static int persistentFloorNumber = 1;
    private static int persistentSceneCycleIndex;

    [Header("Wajib - Puzzle Scene Ini")]
    [Tooltip("Isi dengan manager puzzle di scene ini (harus implement IPuzzleRound), contoh: Puzzle1Manager atau Puzzle2Manager.")]
    [SerializeField] private MonoBehaviour scenePuzzleController;

    [Header("Opsional - Sistem Tambahan")]
    [Tooltip("Kosongkan jika scene ini tidak pakai random spawn item.")]
    [SerializeField] private RunRandomizer runRandomizer;
    [Tooltip("Kosongkan jika scene ini tidak pakai pressure/retak lantai.")]
    [SerializeField] private FloorPressureManager floorPressureManager;

    [Header("Endless - Pindah Scene")]
    [Tooltip("Aktifkan untuk mode endless antar scene puzzle.")]
    [SerializeField] private bool endlessMode = true;
    [Tooltip("Urutan scene endless. Nama harus sama persis dengan scene di Build Settings.")]
    [SerializeField] private string[] endlessSceneNames;

    [Header("Restart")]
    [Tooltip("Tekan R untuk restart run dari lantai 1.")]
    [SerializeField] private bool allowRestartWithR = true;

    private IPuzzleRound cachedScenePuzzle;

    public int RoundNumber => persistentFloorNumber;
    public int CurrentFloorNumber => persistentFloorNumber;
    public int CurrentPuzzleIndex => persistentSceneCycleIndex;
    public string CurrentPuzzleName => GetCurrentPuzzleName();

    public enum GameState
    {
        Playing,
        Win,
        Lose
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public RunRandomizer Randomizer => runRandomizer;
    public FloorPressureManager FloorPressure => floorPressureManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Lebih dari satu game manager ditemukan di scene.");
        }

        Instance = this;

        if (scenePuzzleController != null)
        {
            cachedScenePuzzle = scenePuzzleController as IPuzzleRound;
        }

        if (runRandomizer == null)
        {
            runRandomizer = GetComponent<RunRandomizer>();
        }

        if (floorPressureManager == null)
        {
            floorPressureManager = GetComponent<FloorPressureManager>();
        }

        if (cachedScenePuzzle == null)
        {
            cachedScenePuzzle = FindScenePuzzleController();
        }

        if (cachedScenePuzzle == null)
        {
            Debug.LogWarning("Scene puzzle controller belum valid. Isi field Scene Puzzle Controller dengan script yang implement IPuzzleRound.");
        }
    }

    private void OnValidate()
    {
        if (scenePuzzleController != null && !(scenePuzzleController is IPuzzleRound))
        {
            Debug.LogWarning("Scene Puzzle Controller harus implement IPuzzleRound.");
        }
    }

    private void Start()
    {
        InitializePersistentRunIfNeeded();
        BeginCurrentSceneRound();
    }

    private void Update()
    {
        if (!allowRestartWithR || Keyboard.current == null)
        {
            return;
        }

        if (!Keyboard.current.rKey.wasPressedThisFrame)
        {
            return;
        }

        RestartRun();
    }

    public void WinRun()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        if (!endlessMode)
        {
            CurrentState = GameState.Win;
            Debug.Log("WIN: objective selesai dan berhasil keluar.");
            return;
        }

        persistentFloorNumber++;

        if (endlessSceneNames != null && endlessSceneNames.Length > 0)
        {
            persistentSceneCycleIndex = GetNextSceneIndex();
            string nextSceneName = endlessSceneNames[persistentSceneCycleIndex];

            Debug.Log("Lantai selesai. Pindah ke scene berikutnya: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        Debug.Log("Lantai selesai. Daftar endlessSceneNames kosong, scene tidak berpindah.");
        BeginCurrentSceneRound();
    }

    public void LoseRun()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Lose;
        Debug.Log("LOSE: player gagal bertahan.");
    }

    public void RestartRun()
    {
        ResetPersistentRun();

        if (endlessSceneNames != null && endlessSceneNames.Length > 0)
        {
            SceneManager.LoadScene(endlessSceneNames[0]);
            return;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public bool IsCurrentPuzzleCompleted()
    {
        return cachedScenePuzzle == null || cachedScenePuzzle.IsCompleted;
    }

    private void InitializePersistentRunIfNeeded()
    {
        if (!persistentRunInitialized)
        {
            persistentRunInitialized = true;
            persistentFloorNumber = 1;
        }

        persistentSceneCycleIndex = ResolveCurrentSceneCycleIndex();
    }

    private void BeginCurrentSceneRound()
    {
        CurrentState = GameState.Playing;

        if (cachedScenePuzzle == null)
        {
            cachedScenePuzzle = FindScenePuzzleController();
        }

        if (cachedScenePuzzle != null)
        {
            cachedScenePuzzle.ResetPuzzleRound();
            cachedScenePuzzle.BeginPuzzleRound();
        }

        if (runRandomizer != null)
        {
            runRandomizer.RandomizeRun();
        }

        if (floorPressureManager != null)
        {
            floorPressureManager.PrepareForRound(persistentFloorNumber);
        }

        Debug.Log("Mulai Lantai " + persistentFloorNumber + " - " + GetCurrentPuzzleName());
    }

    private int ResolveCurrentSceneCycleIndex()
    {
        if (endlessSceneNames == null || endlessSceneNames.Length == 0)
        {
            return 0;
        }

        string activeSceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < endlessSceneNames.Length; i++)
        {
            if (endlessSceneNames[i] == activeSceneName)
            {
                return i;
            }
        }

        return 0;
    }

    private int GetNextSceneIndex()
    {
        if (endlessSceneNames == null || endlessSceneNames.Length == 0)
        {
            return 0;
        }

        return (persistentSceneCycleIndex + 1) % endlessSceneNames.Length;
    }

    private string GetCurrentPuzzleName()
    {
        if (endlessSceneNames != null && endlessSceneNames.Length > 0)
        {
            return endlessSceneNames[persistentSceneCycleIndex];
        }

        return SceneManager.GetActiveScene().name;
    }

    private IPuzzleRound FindScenePuzzleController()
    {
        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < behaviours.Length; i++)
        {
            IPuzzleRound puzzleRound = behaviours[i] as IPuzzleRound;
            if (puzzleRound != null)
            {
                return puzzleRound;
            }
        }

        return null;
    }

    private void ResetPersistentRun()
    {
        persistentRunInitialized = true;
        persistentFloorNumber = 1;
        persistentSceneCycleIndex = 0;
    }
}
